using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNETMethodGenerator
{
    public partial class Form1 : Form
    {

        private class UArg
        {
            public string typeName;
            public string argName;
        }

        public Form1()
        {
            InitializeComponent();
            MakeItHappen();
        }

        private void MakeItHappen()
        {
            string methodHead = textBox_methodHead.Text;
            int klammerStartIndex = methodHead.IndexOf('(');
            int klammerEndIndex = methodHead.IndexOf(')');
            if (klammerStartIndex > 0 && klammerEndIndex > klammerStartIndex)
            {
                string preKlammerText = methodHead.Substring(0, klammerStartIndex);
                string[] preKlammerWords = preKlammerText.Split(" ".ToCharArray());
                if(preKlammerWords.Length>1)
                {
                    string methodName = preKlammerWords[preKlammerWords.Length - 1];
                    string methodModifier = "";
                    for(int i=0;i<preKlammerWords.Length-1;i++)
                    {
                        methodModifier += preKlammerWords[i];
                        if (i < preKlammerWords.Length - 2)
                            methodModifier += " ";
                    }

                    string betweenBracketsText = methodHead.Substring(klammerStartIndex + 1, klammerEndIndex - klammerStartIndex - 1);
                    string[] argstexts = betweenBracketsText.Split(",".ToCharArray());
                    List<UArg> args = new List<UArg>();
                    
                    foreach (string argtext in argstexts)
                    {
                        string[] argsWords = argtext.Split(" ".ToCharArray());
                        if(argsWords.Length>=2)
                        {
                            UArg arg = new UArg();
                            arg.typeName = argsWords[argsWords.Length - 2];
                            arg.argName = argsWords[argsWords.Length - 1];
                            args.Add(arg);
                        }
                    }
                    string argsCallNames = "";
                    for(int i=0;i<args.Count; i++)
                    {
                        argsCallNames += args[i].argName;
                        if (i < args.Count - 1)
                            argsCallNames += ",";
                    }

                    //Jetzt werden die Methoden generiert

                    StringBuilder sb = new StringBuilder();

                    //Network Method
                    sb.Append(methodModifier);
                    sb.Append(' ');
                    sb.Append(methodName);
                    sb.Append("Network(");
                    sb.Append(betweenBracketsText);
                    sb.Append(")\r\n{\r\n");
                    sb.Append("if (isLocalPlayer)\r\n");
                    sb.Append("Cmd").Append(methodName).Append('(').Append(argsCallNames).Append(");\r\n");
                    sb.Append("else\r\n{\r\nif(isServer)\r\n");
                    sb.Append("Rpc").Append(methodName).Append('(').Append(argsCallNames).Append(");\r\n}\r\n");
                    sb.Append("Final").Append(methodName).Append('(').Append(argsCallNames).Append(");\r\n}\r\n\r\n");

                    //Command Method
                    sb.Append("[Command]\r\n");
                    sb.Append("private void ");
                    sb.Append("Cmd").Append(methodName).Append('(').Append(betweenBracketsText);
                    sb.Append(")\r\n{\r\n");
                    sb.Append("if (!isLocalPlayer)\r\n");
                    sb.Append("Final").Append(methodName).Append('(').Append(argsCallNames).Append(");\r\n");
                    sb.Append("Rpc").Append(methodName).Append('(').Append(argsCallNames).Append(");\r\n}\r\n\r\n");

                    //Rpc Method
                    sb.Append("[ClientRpc]\r\n");
                    sb.Append("private void ");
                    sb.Append("Rpc").Append(methodName).Append('(').Append(betweenBracketsText);
                    sb.Append(")\r\n{\r\n");
                    sb.Append("if (isServer || isLocalPlayer)\r\nreturn;\r\n");
                    sb.Append("Final").Append(methodName).Append('(').Append(argsCallNames).Append(");\r\n}\r\n\r\n");

                    //Final Method
                    sb.Append("private void ");
                    sb.Append("Final").Append(methodName).Append('(').Append(betweenBracketsText);
                    sb.Append(")\r\n{\r\n");
                    sb.Append("//Your code here\r\n");
                    sb.Append("}\r\n");

                    textBox_output.Text = sb.ToString();
                }
            }

        }

        private void textBox_methodHead_TextChanged(object sender, EventArgs e)
        {
            MakeItHappen();
        }

        


    }
}
