using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChemCompLogger.view
{
    class AppConsole
    {
        private System.Windows.Controls.TextBox textBox;

        private static FileVersionInfo fv = System.Diagnostics.FileVersionInfo.GetVersionInfo
                               (Assembly.GetExecutingAssembly().Location);

        internal static string mazeGeneratorVersion = "0.1";

        private string welcomeText = @"
  -------------------------------
        :--:--:--:--:--:--:
           :--:--:--:--:
             :--:-:--:
                | |
          Maze Generator
                | |
        :--:--:--:--:--:--:

        V {0}

        :--:--:--:--:--:--:
               :-:-:
                :-:
                 |

    Hotkeys: F1-F4 toggle preset window layouts.
    Escape exits the utility from non-table editing views.

    Creating a puzzle requires a modern web-browser installed
    for viewing / printing.

    The binary path for this utility must be user-writable for
    the database.

    Built using (w/o official permission but using free software):
    - C#/XAML/WPF
        - https://docs.microsoft.com/en-us/dotnet/framework/wpf/
    - YamlDotNet by Antoine Aubry
        - https://aaubry.net/pages/yamldotnet.html

    Created by Walter Macfarland
                |
            ---:-:
            /
    ----:-:
    /
:-:
";

        // constructor
        public AppConsole(ref System.Windows.Controls.TextBox textBox)
        {
            var versionSplitByPeriods = mazeGeneratorVersion.Split(new char[] { '.' });
            Int32 majorVersionVal;
            var majorVersion = Int32.TryParse((System.String)versionSplitByPeriods.GetValue(0), out majorVersionVal);
            if (majorVersionVal < 1)
            {
                Int32 minorVersionVal;
                var minorVersion = Int32.TryParse((System.String)versionSplitByPeriods.GetValue(1), out minorVersionVal);
                if (minorVersionVal > 8)
                {
                    mazeGeneratorVersion += " (BETA)";
                }
                else
                {
                    mazeGeneratorVersion += " (ALPHA)";
                }
            }
            this.textBox = textBox;
            Write(string.Format(welcomeText, mazeGeneratorVersion));
        }

        public void Write(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            this.textBox.AppendText(s.Trim() + Environment.NewLine + Environment.NewLine);
            this.textBox.ScrollToEnd();
        }

        public void AddLineBreak()
        {
            this.textBox.AppendText(Environment.NewLine);
            this.textBox.ScrollToEnd();
        }
    }
}
