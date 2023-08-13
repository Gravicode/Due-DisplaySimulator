using DueDisplaySimulator.Helpers;
using GHIElectronics.DUELink;
using GHIElectronics.TinyCLR.DUE;
using System.Runtime.CompilerServices;

namespace DueDisplaySimulator
{
    public partial class Form1 : Form
    {
        ExecuteMode Mode = ExecuteMode.Simulator;
        public enum ExecuteMode
        {
            Simulator, Device
        }
        ScriptEngine script;
        StringConsole desktopConsole;
        string ExecutedCodeResult;
        bool Loading = false;
        public Form1()
        {
            InitializeComponent();
            Setup();
        }

        void Setup()
        {
            TxtInput.Text = """
                            var x = 1
                            while x <= 20
                                if x % 2 != 0
                                    PrintLn(x)
                                end
                                x = x + 1
                            end
                            LcdConfig(0x3C,0,0,0)
                            LcdClear(0)
                            LcdText("Hello World",1,10,10)
                            LcdShow()
                            """;
            BtnExecute.Click += async (a, b) =>
            {
                if (Loading)
                {
                    TxtStatus.Text = "please wait, the script is on process.";
                    return;
                }
                var cmd = TxtInput.Text;
                if (string.IsNullOrEmpty(cmd))
                {
                    TxtStatus.Text = "please input due script first";
                    return;
                }

                await ExecuteCode(cmd);


            };
            BtnClear.Click += (a, b) =>
            {
                TxtInput.Clear();
                TxtConsole.Clear();
                ExtensionLibrary.initGfx();
                DisplayBox.Image = ExtensionLibrary.GetScreen();
                script.Reset();
                TxtStatus.Text = "reset ok";
            };
            CmbSelectDevice.DataSource = new List<string>() { "Simulator", "Device" };
            CmbSelectDevice.SelectedIndex = 0;
            CmbSelectDevice.SelectedIndexChanged += (a, b) =>
            {
                Mode = CmbSelectDevice.SelectedIndex == 0 ? ExecuteMode.Simulator : ExecuteMode.Device;
            };
            script = new ScriptEngine(typeof(ExtensionLibrary));
            desktopConsole = new StringConsole();
            ExtensionLibrary.Print += (a, teks) =>
            {
                desktopConsole.Print(teks);
            };
            script.SetConsole(desktopConsole);
        }

        async Task<bool> ExecuteCode(string command)
        {
            Loading = true;
            if (Mode == ExecuteMode.Device)
            {
                try
                {


                    var port = DUELinkController.GetConnectionPort();

                    var dev = new DUELinkController(port);

                    var res = dev.Script.Execute(command);

                    dev.Disconnect();

                    if (res)
                    {
                        StatusLbl.Text = "Code has been executed successfully";
                    }
                    else
                    {
                        StatusLbl.Text = "Code failed to execute";
                    }

                    return res;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"code failed to execute: {ex}");
                    return false;
                }
                finally
                {
                    Loading = false;
                }
            }
            else
            {
                try
                {

                    script.Run(command);
                    ExecutedCodeResult = desktopConsole.GetContent();
                    TxtConsole.Text = ExecutedCodeResult;
                    var scr = ExtensionLibrary.GetScreen();
                    if (scr != null)
                    {
                        DisplayBox.Image = scr;
                        DisplayBox.Refresh();
                    }
                    StatusLbl.Text = "Code has been loaded successfully";
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show( $"Code failed to load: {ex}");
                    Console.WriteLine(ex);
                }
                finally
                {
                    Loading = false;
                }

                return false;
            }

        }
    }
}