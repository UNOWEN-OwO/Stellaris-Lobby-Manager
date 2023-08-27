using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Stellaris_Lobby_Manager
{
    public partial class lobbyManager : Form
    {
        private const string STELLARIS_PROCESS_NAME = "stellaris";

        private static Process? stellarisProcess;

        private static IntPtr lobbyAddress = IntPtr.Zero;
        private static IntPtr gameBase = IntPtr.Zero;
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private static bool lobbyModified = false;

        private static Dictionary<string, SettingItem> lobbySettings = new Dictionary<string, SettingItem>();
        private static Dictionary<string, SettingItem> _lobbySettings = new Dictionary<string, SettingItem>();

        private static Dictionary<string, string> galaxySizeDefault = new Dictionary<string, string>();
        private static Dictionary<string, string> galaxyShapeDefault = new Dictionary<string, string>();

        // UNLOCK:"" LOCK:"" RETURN:""
        private static readonly List<string> controlSymbol = new List<string> { "", "", "" };

        private static byte[]? overflowRestore;

        public lobbyManager()
        {
            _SetLanguage(Properties.Settings.Default.language);
            _InitializeComponent();
            GenerateLobbySettings();
            // SetLanguage(Properties.Settings.Default.language);
            MapSettings(AppdataFile(Properties.Settings.Default.lobbyName + ".json"));

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Task.Run(() => GetStellaris(), cancellationTokenSource.Token);
        }

        private void _InitializeComponent()
        {
            InitializeComponent();
            englishToolStripMenuItem.Click += (_, _) => { SetLanguage(0); };
            chineseToolStripMenuItem.Click += (_, _) => { SetLanguage(1); };
            fullPrecisionToolStripMenuItem.Checked = Properties.Settings.Default.fullPrecision;
            canOverflowToolStripMenuItem.Checked = Properties.Settings.Default.canOverflow;
            currentLobby.Text = Properties.Settings.Default.lobbyName;
            LoadLobbyList();
        }

        private string appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stellaris-Lobby-Manager");
        private string AppdataFile(string file) => Path.Combine(appdata, file);


        private void GetStellaris()
        {

            /// <summary>
            /// task loop that search Stellaris process and get the lobby address
            /// </summary>

            Debug.WriteLine("Waiting for Stellaris process");
            while (true)
            {
                stellarisProcess = Process.GetProcessesByName(STELLARIS_PROCESS_NAME).FirstOrDefault();
                IntPtr lobbyBase = IntPtr.Zero;
                if (!(stellarisProcess == null || stellarisProcess.HasExited))
                {
                    Debug.WriteLine("Stellaris process found");

                    stellarisProcess.EnableRaisingEvents = true;
                    stellarisProcess.Exited += (sender, args) =>
                    {
                        stellarisProcess = null;
                        lobbyAddress = lobbyBase = IntPtr.Zero;
                    };
                    gameBase = MemoryHelper.GetModuleBaseAddress(stellarisProcess, "stellaris.exe");
                    MatchOffset();
                    _ToggleOverflow(Properties.Settings.Default.canOverflow);

                    int offset1 = Properties.Settings.Default.lobbyOffset1;

                    while (stellarisProcess != null && offset1 == Properties.Settings.Default.lobbyOffset1)
                    {

                        /* Write galaxy size
                         * stellaris.exe+164E2C - 48 8D 71 10           - lea rsi,[rcx+10]
                         * stellaris.exe+164E30 - 48 8D 7A 10           - lea rdi,[rdx+10]
                         * ->   stellaris.exe+164E34 - 48 89 41 08           - mov [rcx+08],rax
                         * stellaris.exe+164E38 - 48 3B F7              - cmp rsi,rdi
                         * stellaris.exe+164E3B - 0F84 BD000000         - je stellaris.exe+164EFE
                         */

                        if ((lobbyBase = MemoryHelper.Read<IntPtr>(stellarisProcess, gameBase +
                            // 0x2772368
                            Properties.Settings.Default.lobbyOffset1)) == IntPtr.Zero)
                        {
                            lobbyAddress = IntPtr.Zero;
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            var _lobbyAddress = MemoryHelper.Read<IntPtr>(stellarisProcess, lobbyBase +
                                // 0x8C8
                                Properties.Settings.Default.lobbyOffset2);
                            if (IsDisposed)
                            {
                                return;
                            }
                            if (lobbyAddress != _lobbyAddress)
                            {
                                lobbyAddress = _lobbyAddress;
                                Debug.WriteLine("Generate Lobby Settings");
                                Action _GenerateGalaxySettings = delegate
                                {
                                    GenerateGalaxySettings();
                                    MapSettings(AppdataFile(Properties.Settings.Default.lobbyName + ".json"));
                                };
                                Invoke(_GenerateGalaxySettings);
                            }
                            // Debug.WriteLine("Fallen Count: " + MemoryHelper.Read<int>(stellarisProcess, _lobbyAddress + 0x58).ToString());
                            Action _GetGameSettings = delegate { GetGameSettings(); };
                            Invoke(_GetGameSettings);
                            Thread.Sleep(1000);
                        }
                    }
                }
                Thread.Sleep(10000);
            }
        }

        private void MatchOffset()
        {
            if (stellarisProcess == null)
                return;

            // check current path
            if (File.Exists("offsets.json"))
            {
                // get game timestamp
                string timestamp = MemoryHelper.Read<int>(stellarisProcess, gameBase + MemoryHelper.Read<int>(stellarisProcess, gameBase + 0x3C) + 0x8).ToString();
                // load offsets from file
                var offsets = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(File.ReadAllText("offsets.json"));
                // check if offsets match
                if (offsets != null && offsets.ContainsKey(timestamp))
                {
                    // load offsets
                    foreach (var item in offsets[timestamp])
                    {
                        Properties.Settings.Default[item.Key] = item.Value;
                    }
                    Properties.Settings.Default.Save();
                    return;
                }
            }
        }

        private void GetGameSettings()
        {
            if (stellarisProcess == null)
                return;
            // MethodInfo mi = typeof(MemoryHelper).GetMethod("Read", BindingFlags.Public | BindingFlags.Static);
            foreach (var item in lobbySettings.Values)
            {
                // MethodInfo methodInfo = mi.MakeGenericMethod(item.valueType == typeof(bool) ? typeof(byte) : item.valueType);
                // var value = methodInfo.Invoke(null, new object[] { stellarisProcess, lobbyAddress + item.offset });
                var value = MemoryHelper.Read(stellarisProcess, lobbyAddress + item.Offset, item.ValueType == typeof(bool) ? typeof(byte) : item.ValueType);
                if (item is GalaxySettingItem)
                {
                    var _item = (GalaxySettingItem)item;
                    string str = MemoryHelper.ReadString(stellarisProcess, (IntPtr)value + _item.StrOffset);
                    value = _item.Options[str].Item1;
                }
                else if (item is YearSettingItem)
                {
                    var _item = (YearSettingItem)item;
                    value = Convert.ToInt32(value) == -1 ? _item.DefaultValue : Convert.ToDecimal(value) + 2200;
                }
                else if (item is CheckSettingItem)
                {
                    value = Convert.ToBoolean(value);
                }

                if (value != item.ValueGame || item.ValueGame != item.ValueSet)
                {
                    if (item.IsLocked)
                    {
                        if (item.ValueGame != item.ValueSet)
                        {
                            item.ValueGame = item.ValueSet;
                        }

                        var _value = item.ValueGame;

                        if (item is YearSettingItem)
                        {
                            value = Convert.ToInt32(_value) - 2200;
                        }
                        else if (item is CheckSettingItem)
                        {
                            value = Convert.ToByte(_value);
                        }
                        else if (item is GalaxySettingItem)
                        {
                            value = ((GalaxySettingItem)item).GetOptionPtrByInt(Convert.ToInt32(_value));
                        }
                        else
                        {
                            value = Convert.ChangeType(_value, item.ValueType);
                        }
                        item.ValueGame = value;
                        MemoryHelper.Write(stellarisProcess, lobbyAddress + item.Offset, value);
                    }
                    else if (value != item.ValueGame)
                    {
                        SetControlValue(item, true, value);
                    }
                }
            }
        }

        private void _SetLanguage(int language)
        {
            switch (language)
            {
                case 1:
                    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh");
                    break;
                default:
                    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
                    break;
            }
        }

        private void SetLanguage(int language)
        {
            _SetLanguage(language);
            Properties.Settings.Default.language = language;
            Properties.Settings.Default.Save();
            ReloadApp();
        }

        private void ReloadApp()
        {
            Controls.Clear();
            _InitializeComponent();
            GenerateDefaultGalaxySettings();

            foreach (SettingItem item in lobbySettings.Values)
            {
                SetControlButton(item);
            }
        }

        private object GetControl(string name) => Controls.Find(name, true).FirstOrDefault();
        private void SetControlValue(SettingItem item, bool isGameValue, object value)
        {
            var control = (Control)GetControl(item.Name + (isGameValue ? "Game" : "Set"));

            if (control.Tag.Equals(0))
            {
                control.Tag = 2;
                if (isGameValue)
                {
                    item.ValueGame = value;
                }
                else
                {
                    item.ValueSet = value;
                }

                if (item is CheckSettingItem)
                {
                    ((CheckBox)control).Checked = Convert.ToBoolean(value);
                }
                else if (item is SelectSettingItem || item is GalaxySettingItem)
                {
                    ((ComboBox)control).SelectedIndex = ((ComboBox)control).Items.Count <= Convert.ToInt32(value) ? -1 : Convert.ToInt32(value);
                }
                else if (item is PreciseNumericSettingItem)
                {
                    ((NumericUpDown)control).Value = Convert.ToDecimal(value) / (decimal)Math.Pow(10, ((NumericUpDown)control).DecimalPlaces);
                }
                else
                {
                    ((NumericUpDown)control).Value = Convert.ToDecimal(value);
                }
                control.Tag = 0;
            }
        }

        // private dynamic getControlValue(SettingItem item, string name)
        // {
        //     // var control = getControl(item.name + (isGameValue == null ? "" : (bool)isGameValue ? "Game" : "Set"));
        //     var control = getControl(name);
        //     return _getControlValue(item, control);
        // }

        private object GetControlValue(SettingItem item, object control)
        {
            if (item is CheckSettingItem)
            {
                return ((CheckBox)control).Checked;
            }
            else if (item is SelectSettingItem || item is GalaxySettingItem)
            {
                return ((ComboBox)control).SelectedIndex;
            }
            else if (item is PreciseNumericSettingItem)
            {
                return ((NumericUpDown)control).Value * (decimal)Math.Pow(10, ((NumericUpDown)control).DecimalPlaces);
            }
            else
            {
                return ((NumericUpDown)control).Value;
            }
        }

        private bool MapSettings(string fileName)
        {
            try
            {
                string json;
                // read json
                if (!File.Exists(fileName))
                {
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Stellaris_Lobby_Manager.default.json");
                    using (var reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }
                    currentLobby.Text = "default";
                }
                else
                {
                    json = File.ReadAllText(fileName);
                }

                // deserialize json
                var settings = JsonConvert.DeserializeObject<Dictionary<string, SettingItem>>(json);
                // map to lobbySettings
                foreach (var item in settings)
                {
                    var _item = lobbySettings[item.Key];
                    _item.IsLocked = item.Value.IsLocked;
                    _item.IsSkipped = item.Value.IsSkipped;
                    if (_item is GalaxySettingItem)
                    {
                        _item.Value = ((GalaxySettingItem)_item).Options[item.Value.Value.ToString()].Item1;
                    }
                    else
                    {
                        _item.Value = item.Value.Value;
                    }
                    SetControlButton(_item);
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Called once
        private void GenerateLobbySettings()
        {
            Debug.WriteLine("Generating lobby settings");

            lobbySettings = LobbySettings.defaultSettings();

            GenerateDefaultGalaxySettings();

            foreach (var item in lobbySettings.Values)
            {
                // change numericUpDown precision
                if (item is PreciseNumericSettingItem)
                {
                    var _item = (PreciseNumericSettingItem)item;
                    ((NumericUpDown)GetControl(item.Name + "Set")).DecimalPlaces = ((NumericUpDown)GetControl(item.Name + "Game")).DecimalPlaces = _item.FullPrecision > 0 && Properties.Settings.Default.fullPrecision ? _item.FullPrecision : _item.DefaultPrecision;
                }
                else if (item is GalaxySettingItem)
                {
                    var _item = (GalaxySettingItem)item;
                    _item.Options = _item.IsGalaxySize ? galaxySizeDefault.Keys.Select((x, i) => new { x, i }).ToDictionary(x => x.x, x => (x.i, IntPtr.Zero))
                                                       : galaxyShapeDefault.Keys.Select((x, i) => new { x, i }).ToDictionary(x => x.x, x => (x.i, IntPtr.Zero));
                }

                // record alias
                _lobbySettings.Add(item.Name + "Game", item);
                _lobbySettings.Add(item.Name + "Set", item);
                _lobbySettings.Add(item.Name + "Apply", item);
                _lobbySettings.Add(item.Name + "Record", item);
                _lobbySettings.Add(item.Name + "Control", item);
            }
        }

        private void GenerateDefaultGalaxySettings()
        {
            // get localizeation string
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            ComponentResourceManager resxSet = new ComponentResourceManager(typeof(Res));

            galaxySizeDefault.Clear();
            galaxyShapeDefault.Clear();
            foreach (string name in LobbySettings.GalaxySizeSrc)
            {
                galaxySizeDefault[name] = resxSet.GetString($"galaxySize.{currentCulture.Name}.{name}") ?? resxSet.GetString($"galaxySize.{name}") ?? name;
            }
            foreach (string name in LobbySettings.GalaxyShapeSrc)
            {
                galaxyShapeDefault[name] = resxSet.GetString($"galaxyShape.{currentCulture.Name}.{name}") ?? resxSet.GetString($"galaxyShape.{name}") ?? name;
            }

            galaxySizeGame.Items.AddRange(galaxySizeDefault.Values.ToArray());
            galaxySizeSet.Items.AddRange(galaxySizeDefault.Values.ToArray());
            galaxyShapeGame.Items.AddRange(galaxyShapeDefault.Values.ToArray());
            galaxyShapeSet.Items.AddRange(galaxyShapeDefault.Values.ToArray());
        }

        private void GenerateGalaxySettings()
        {
            Debug.WriteLine("Generating galaxy settings");

            GalaxySettingItem galaxySize = (GalaxySettingItem)lobbySettings["galaxySize"];
            GalaxySettingItem galaxyShape = (GalaxySettingItem)lobbySettings["galaxyShape"];

            /* Galaxy Size Offset2
             * stellaris.exe+F0DBBE - 48 8B B8 C8080000     - mov rdi,[rax+000008C8]
             * stellaris.exe+F0DBC5 - 48 8B 05 8C2C9801     - mov rax,[stellaris.exe+2890858] { (2386EDFFD10) }
             * ->   stellaris.exe+F0DBCC - 48 8B 48 18           - mov rcx,[rax+18]
             * stellaris.exe+F0DBD0 - 48 63 40 24           - movsxd  rax,dword ptr [rax+24]
             * stellaris.exe+F0DBD4 - 48 8D 14 C1           - lea rdx,[rcx+rax*8]
             */

            IntPtr _sizeBase = MemoryHelper.Read<IntPtr>(stellarisProcess,
                               MemoryHelper.GetModuleBaseAddress(stellarisProcess, "stellaris.exe") +
                               // 0x2890858
                               Properties.Settings.Default.sizeOffset1);
            IntPtr sizeBase = MemoryHelper.Read<IntPtr>(stellarisProcess, _sizeBase +
                              // 0x18
                              Properties.Settings.Default.sizeOffset2);

            int sizeCount = MemoryHelper.Read<int>(stellarisProcess, _sizeBase + Properties.Settings.Default.sizeCountOffset);

            if (sizeCount <= 0 || sizeCount > 30)
            {
                Debug.WriteLine("Invalid galaxy size count");
                return;
            }

            galaxySize.Options.Clear();
            galaxyShape.Options.Clear();

            galaxySizeGame.Items.Clear();
            galaxySizeSet.Items.Clear();
            galaxyShapeGame.Items.Clear();
            galaxyShapeSet.Items.Clear();

            List<string> sizeList = new List<string>();
            int maxShapeIdx = 0, maxShapeCount = 0;
            // 0x24
            for (int i = 0; i < MemoryHelper.Read<int>(stellarisProcess, _sizeBase + Properties.Settings.Default.sizeCountOffset); i++)
            {
                IntPtr sizePtr = MemoryHelper.Read<IntPtr>(stellarisProcess, sizeBase + i * 8);
                string sizeStr = MemoryHelper.ReadString(stellarisProcess, sizePtr + Properties.Settings.Default.sizeStringOffset);
                // galaxySizeItem[Enum.Parse<GalaxySize>(sizeStr)] = sizePtr;
                sizeList.Add(galaxySizeDefault.ContainsKey(sizeStr) ? galaxySizeDefault[sizeStr] : sizeStr);
                galaxySize.Options[sizeStr] = (i, sizePtr);
                int shapeCount = MemoryHelper.Read<int>(stellarisProcess, sizePtr + Properties.Settings.Default.shapeOffset + 0xC);
                if (shapeCount > maxShapeCount)
                {
                    maxShapeCount = shapeCount;
                    maxShapeIdx = i;
                }
            }
            galaxySizeGame.Items.AddRange(sizeList.ToArray());
            galaxySizeSet.Items.AddRange(sizeList.ToArray());

            /* Galaxy Shape
             * stellaris.exe+F0E499 - 4C 8B B0 C8080000     - mov r14,[rax+000008C8]
             * stellaris.exe+F0E4A0 - 49 8B 7E 08           - mov rdi,[r14+08]
             * ->   stellaris.exe+F0E4A4 - 48 8B 07              - mov rax,[rdi]
             * stellaris.exe+F0E4A7 - 48 8B CF              - mov rcx,rdi
             */
            List<string> shapeList = new List<string>();
            // just assume galaxyShape doesn't share same keyword with galaxySize
            for (int i = 0; i < maxShapeCount; i++)
            {
                IntPtr shapePtr = MemoryHelper.Read<IntPtr>(stellarisProcess,
                                  MemoryHelper.Read<IntPtr>(stellarisProcess,
                                  MemoryHelper.Read<IntPtr>(stellarisProcess,
                                  // 1000 Stars supports most galaxy shapes
                                  sizeBase + maxShapeIdx * 0x8) +
                                  // 0x170
                                  Properties.Settings.Default.shapeOffset) +
                                  i * 8);
                string shapeStr = MemoryHelper.ReadString(stellarisProcess, shapePtr + Properties.Settings.Default.shapeStringOffset);
                // galaxyShapeItem[Enum.Parse<GalaxyShape>(shapeStr)] = shapePtr;
                shapeList.Add(galaxyShapeDefault.ContainsKey(shapeStr) ? galaxyShapeDefault[shapeStr] : shapeStr);
                galaxyShape.Options[shapeStr] = (i, shapePtr);
            }
            galaxyShapeGame.Items.AddRange(shapeList.ToArray());
            galaxyShapeSet.Items.AddRange(shapeList.ToArray());
        }

        /* Overflow check
         * stellaris.exe+171F5A - 48 8D 54 24 20        - lea rdx,[rsp+20]
         * stellaris.exe+171F5F - 48 8B 8F C8080000     - mov rcx,[rdi+000008C8]
         * ->   stellaris.exe+171F66 - E8 C5000000           - call stellaris.exe+172030
         * stellaris.exe+171F6B - 90                    - nop 
         */
        private void ToggleOverflow(object sender, EventArgs e)
        {
            Properties.Settings.Default.canOverflow = !Properties.Settings.Default.canOverflow;
            Properties.Settings.Default.Save();
            _ToggleOverflow(Properties.Settings.Default.canOverflow);
        }

        private void _ToggleOverflow(bool canOverflow)
        {
            if (stellarisProcess == null)
                return;

            if (canOverflow)
            {
                overflowRestore = MemoryHelper.ReadUnmanaged(stellarisProcess, gameBase + Properties.Settings.Default.overflowOffset, 5);

                // in case program is closed before overflow is restored
                if (overflowRestore.SequenceEqual(new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }))
                {
                    overflowRestore = new byte[] { 0xE8, 0xC5, 0x00, 0x00, 0x00 };
                }
                MemoryHelper.WriteUnmanaged(stellarisProcess, gameBase + Properties.Settings.Default.overflowOffset, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 });
            }
            else if (overflowRestore != null)
            {
                MemoryHelper.WriteUnmanaged(stellarisProcess, gameBase + Properties.Settings.Default.overflowOffset, overflowRestore);
            }
        }

        private void OnProcessExit(object? sender, EventArgs e)
        {
            if (stellarisProcess != null && !stellarisProcess.HasExited && Properties.Settings.Default.canOverflow)
            {
                _ToggleOverflow(false);
            }
        }

        private void TogglePrecision(object sender, EventArgs e)
        {
            Properties.Settings.Default.fullPrecision = !Properties.Settings.Default.fullPrecision;
            Properties.Settings.Default.Save();
            foreach (var item in lobbySettings.Values)
            {
                if (item is PreciseNumericSettingItem)
                {
                    var _item = (PreciseNumericSettingItem)item;
                    ((NumericUpDown)GetControl(_item.Name + "Set")).DecimalPlaces = ((NumericUpDown)GetControl(_item.Name + "Game")).DecimalPlaces = Properties.Settings.Default.fullPrecision ? _item.FullPrecision : _item.DefaultPrecision;
                }
            }
        }

        private void OnFocus(object sender, EventArgs e)
        {
            ((Control)sender).Tag = 1;
        }

        private void onClose(object sender, EventArgs e)
        {
            ((Control)sender).Tag = 0;
        }

        private void OnLeave(object sender, EventArgs e)
        {
            ActiveControl = null;
            onClose(sender, e);
        }

        private void GalaxySettingsChanged(object sender, EventArgs e)
        {
            var control = (ComboBox)sender;
            control.Tag = 1;
            GalaxySettingItem item = (GalaxySettingItem)_lobbySettings[control.Name];
            var index = control.SelectedIndex;
            if (stellarisProcess == null)
            {
                item.ValueGame = index;
            }
            else if (Convert.ToInt32(item.ValueGame) != index)
            {
                item.ValueGame = index;
                MemoryHelper.Write(stellarisProcess, lobbyAddress + item.Offset, item.GetOptionPtrByInt(index));
            }
            control.Tag = 0;
        }

        private void OnGameValueChanged(object sender, EventArgs e)
        {
            var control = (Control)sender;
            control.Tag = 1;
            string name = control.Name;
            SettingItem item = _lobbySettings[name];
            var value = GetControlValue(item, control);
            if (stellarisProcess == null)
            {
                item.ValueGame = value;
            }
            else if (item.ValueGame != value)
            {
                item.ValueGame = value;
                if (item is YearSettingItem)
                {
                    value = Convert.ToInt32((decimal)value - 2200);
                }
                else if (item is CheckSettingItem)
                {
                    value = Convert.ToByte(value);
                }
                else
                {
                    value = Convert.ChangeType(value, item.ValueType);
                }
                MemoryHelper.Write(stellarisProcess, lobbyAddress + item.Offset, value);
            }
            control.Tag = 0;
        }

        private void OnSetValueChanged(object sender, EventArgs e)
        {
            var control = (Control)sender;
            SettingItem item = _lobbySettings[control.Name];
            var value = GetControlValue(item, sender);
            if (!lobbyModified || item.ValueSet != value)
            {
                item.ValueSet = value;
                LobbyModifiedChanged(true);
                if (item.IsLocked)
                {
                    SetControlValue(item, true, value);
                }
            }

        }

        private static void CopyControlValue(object c1, object c2)
        {
            /// <summary>
            /// From c1 to c2
            /// </summary>
            if (c1 is ComboBox)
            {
                ((ComboBox)c2).SelectedIndex = ((ComboBox)c1).SelectedIndex;
            }
            else if (c1 is CheckBox)
            {
                ((CheckBox)c2).Checked = ((CheckBox)c1).Checked;
            }
            else
            {
                ((NumericUpDown)c2).Value = ((NumericUpDown)c1).Value;
            }
        }

        private void RecordSingle_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Name;
            SettingItem item = _lobbySettings[name];
            CopyControlValue(GetControl(item.Name + "Game"), GetControl(item.Name + "Set"));

            if (item is NumericRandomSettingItem)
            {
                CopyControlValue(GetControl(item.Name + "RandomGame"), GetControl(item.Name + "RandomSet"));
                var randomItem = lobbySettings[item.Name + "Random"];
                randomItem.ValueGame = randomItem.ValueSet;
            }

            item.ValueSet = item.ValueGame;
            LobbyModifiedChanged(false);
        }

        private void RecordButton_Click(object sender, EventArgs e)
        {
            bool isSame = true;
            foreach (SettingItem item in lobbySettings.Values)
            {
                CopyControlValue(GetControl(item.Name + "Game"), GetControl(item.Name + "Set"));

                item.ValueSet = item.ValueGame;
                isSame &= item.ValueGame == item.ValueSet;
            }
            LobbyModifiedChanged(!isSame);
        }

        private void ApplySingle_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Name;
            SettingItem item = _lobbySettings[name];

            // if (item.IsSkipped) return; // Theroetically, this should not happen as the button should be disabled.
            CopyControlValue(GetControl(item.Name + "Set"), GetControl(item.Name + "Game"));

            if (item is NumericRandomSettingItem)
            {
                CopyControlValue(GetControl(item.Name + "RandomSet"), GetControl(item.Name + "RandomGame"));
                var randomItem = lobbySettings[item.Name + "Random"];
                randomItem.ValueSet = randomItem.ValueGame;
            }

            item.ValueSet = item.ValueGame;
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            foreach (SettingItem item in lobbySettings.Values)
            {
                if (!item.IsSkipped)
                {
                    CopyControlValue(GetControl(item.Name + "Set"), GetControl(item.Name + "Game"));
                    item.ValueSet = item.ValueGame;
                }
            }

        }

        private void SetControlButton(SettingItem item)
        {
            /// <summary>
            /// Use to restore SetControl value & ControlButton status
            /// </summary>
            SetControlValue(item, false, item.Value);

            var btn = (Button)GetControl(item.Name + "Control");
            if (Controls.Find(item.Name + "Control", true).Length == 0) return;

            if (item.IsLocked)
            {
                SetControlValue(item, true, item.Value);
                SwitchControlButton(btn, 1);
            }
            else if (item.IsSkipped)
            {
                SwitchControlButton(btn, 2);
            }
            else
            {
                SwitchControlButton(btn, 0);
            }
        }

        private void SwitchControlNumericRandom(string name, int tag)
        {
            var item = lobbySettings[name];
            var gameControl = (Control)GetControl($"{name}Game");
            var setControl = (Control)GetControl($"{name}Set");

            if (tag == 1)
            {
                item.IsLocked = true;
                item.IsSkipped = false;
                gameControl.Enabled = false;
                setControl.Enabled = true;
                CopyControlValue(setControl, gameControl);
            }
            else if (tag == 2)
            {
                item.IsLocked = false;
                item.IsSkipped = true;
                gameControl.Enabled = true;
                setControl.Enabled = false;
                CopyControlValue(gameControl, setControl);
            }
            else
            {
                item.IsLocked = false;
                item.IsSkipped = false;
                gameControl.Enabled = setControl.Enabled = true;
            }
        }

        private void SwitchControlButton(Button button, int tag)
        {
            var item = _lobbySettings[button.Name];
            button.Tag = tag;
            button.Text = controlSymbol[tag];

            var gameControl = (Control)GetControl($"{item.Name}Game");
            var setControl = (Control)GetControl($"{item.Name}Set");
            var applyButton = (Control)GetControl($"{item.Name}Apply");
            var recordButton = (Control)GetControl($"{item.Name}Record");

            if (tag == 1)
            {
                item.IsLocked = true;
                item.IsSkipped = false;
                gameControl.Enabled = applyButton.Enabled = false;
                setControl.Enabled = recordButton.Enabled = true;
                CopyControlValue(setControl, gameControl);
            }
            else if (tag == 2)
            {
                item.IsLocked = false;
                item.IsSkipped = true;
                gameControl.Enabled = applyButton.Enabled = true;
                setControl.Enabled = recordButton.Enabled = false;
            }
            else
            {
                item.IsLocked = item.IsSkipped = false;
                gameControl.Enabled = setControl.Enabled = applyButton.Enabled = recordButton.Enabled = true;
            }

            if (item is NumericRandomSettingItem)
            {
                SwitchControlNumericRandom(item.Name + "Random", tag);
            }
        }

        private void ControlButton_Click(object sender, EventArgs e)
        {
            // UNLOCK:"" LOCK:"" RETURN:""
            var button = (Button)sender;
            var tag = ((int)button.Tag + 1) % 3;
            SwitchControlButton(button, tag);
        }

        private void LoadLobbyList()
        {
            // get all json in appdata
            if (!Directory.Exists(appdata))
            {
                Directory.CreateDirectory(appdata);
            }
            else
            {
                var files = Directory.GetFiles(appdata, "*.json").Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
                lobbyLibrary.Items.Clear();
                lobbyLibrary.Items.AddRange(files);

                if (files.Contains(Properties.Settings.Default.lobbyName))
                {
                    lobbyLibrary.SelectedItem = Properties.Settings.Default.lobbyName;
                }
                else
                {
                    LobbyModifiedChanged(true);
                }
            }
        }

        private void LobbyLibrary_DoubleClick(object sender, EventArgs e)
        {
            if (lobbyLibrary.SelectedItem != null)
            {
                Debug.WriteLine("Loaded config: " + lobbyLibrary.SelectedItem.ToString());
                if (MapSettings(AppdataFile(lobbyLibrary.SelectedItem.ToString() + ".json")))
                {
                    currentLobby.Text = lobbyLibrary.SelectedItem.ToString();
                    LobbyModifiedChanged(false);
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // save to appdata
            SaveFile(AppdataFile(currentLobby.Text + ".json"));

        }

        private void LoadConfig(string filename)
        {
            string initFilename = Path.GetFileNameWithoutExtension(filename);
            string currentFilename = initFilename;
            int count = 0;

            while (File.Exists(AppdataFile(currentFilename + ".json")))
            {
                count++;
                currentFilename = $"{initFilename}_{count}";
            }

            if (MapSettings(AppdataFile(currentFilename + ".json")))
            {
                File.Copy(filename, AppdataFile(currentFilename + ".json"));
                currentLobby.Text = currentFilename;
                LoadLobbyList();
                LobbyModifiedChanged(false);
            }
        }

        private void DragDropEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void DragDropConfig(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                LoadConfig(files[0]);
            }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "json files (*.json)|*.json";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadConfig(openFileDialog.FileName);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "json files (*.json)|*.json";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = currentLobby.Text + ".json";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveFile(saveFileDialog.FileName);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (lobbyLibrary.SelectedItem != null)
            {
                File.Delete(AppdataFile(lobbyLibrary.SelectedItem.ToString() + ".json"));
                LoadLobbyList();
            }
        }

        private void SaveFile(string path)
        {
            foreach (SettingItem item in lobbySettings.Values)
            {
                item.Value = item.ValueSet;
            }

            var galaxySize = (GalaxySettingItem)lobbySettings["galaxySize"];
            var galaxyShape = (GalaxySettingItem)lobbySettings["galaxyShape"];

            galaxySize.Value = galaxySize.GetOptionStrByInt(Convert.ToInt32(galaxySize.ValueSet));
            galaxyShape.Value = galaxyShape.GetOptionStrByInt(Convert.ToInt32(galaxyShape.ValueSet));

            // JObject settings = (JObject)JToken.FromObject(lobbySettings);
            string json = JsonConvert.SerializeObject(lobbySettings, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

            galaxySize.Value = galaxySize.ValueSet;
            galaxyShape.Value = galaxyShape.ValueSet;

            // string json = JsonConvert.SerializeObject(lobbySettings, Formatting.Indented);
            File.WriteAllText(path, json);

            LobbyModifiedChanged(false);

            LoadLobbyList();
        }

        private void CurrentLobby_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.lobbyName = currentLobby.Text;
            Properties.Settings.Default.Save();
        }

        private void LobbyModifiedChanged(bool modified)
        {
            if (modified ^ lobbyModified)
            {
                if (modified)
                {
                    currentLabel.Text = currentLabel.Text.Insert(currentLabel.Text.Length - 1, "*");
                    currentLobby.Font = new Font(currentLobby.Font, FontStyle.Italic);
                }
                else
                {
                    currentLabel.Text = currentLabel.Text.Replace("*", "");
                    currentLobby.Font = new Font(currentLobby.Font, FontStyle.Regular);
                }
                lobbyModified = modified;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Stellaris Lobby Manager v1.1 Made by @UNOWEN-OwO\nhttps://github.com/UNOWEN-OwO/Setllaris-Lobby-Manager\nVisit Github page?", "About", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.OK)
            {
                Process.Start(new ProcessStartInfo("https://github.com/UNOWEN-OwO/Setllaris-Lobby-Manager") { UseShellExecute = true });
            }
        }

        private void offsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ToggleOverflow(false);
            var offsetForm = new OffsetSetter();
            offsetForm.Show();
            offsetForm.FormClosed += OffsetForm_FormClosed;
        }

        private void OffsetForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            lobbyAddress = IntPtr.Zero;
            ReloadApp();
        }

        private void ChangeGameStatus(GameStatus status)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            ComponentResourceManager resxSet = new ComponentResourceManager(typeof(Res));
            info.Text = resxSet.GetString($"info.{currentCulture.Name}.{status}");
        }
    }
}