using System.Globalization;
using System.Threading;
using InfoReaderPlugin.ExpressionParser;
using Sync.Tools;

namespace InfoReaderPlugin
{
    using InfoReaderPlugin.NewExpressionParser.Tools;
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    partial class InfoReader
    {
        StringBuilder _rawFormat;
        string _replaceStr = "";
        
        private readonly System.IO.MemoryMappedFiles.MemoryMappedFile _outputInfoMap;
        private System.IO.Stream _outputMapStream;
        void RefreshMmf()
        {
            while (true)
            {
                string matchedValue;
                Thread.Sleep(5);
                _outputMapStream = CurrentStatusMmf?.StreamOfMappedFile;
                if(CurrentStatusMmf is null || !CurrentStatusMmf.EnableOutput)
                {
                    continue;
                }
                if (_outputMapStream is null)
                {
                    continue;
                }
                try
                {
                    if (_rawFormat?.ToString() != _fileFormat)
                        _rawFormat = new StringBuilder(_fileFormat ?? "");
                    var variableResult = ExpressionMatchers[ExpressionTypes.Builtin | ExpressionTypes.Variable].Results;
                    var boolResult = ExpressionMatchers[ExpressionTypes.Builtin | ExpressionTypes.If].Results;
                    if (variableResult.Length > 0)
                        foreach (string match in variableResult)
                        {
                            try
                            {
                                /*_matchedValue = match;
                                VariableExpression expression = new VariableExpression(_matchedValue, _ortdpWrapper);

                                _replaceStr = ExpressionTools.CalcRpnExpression(
                                    ExpressionTools.ConvertToRpnExpression(expression.NoFormatExpression), _ortdpWrapper);
                                if (expression.HasFormat)
                                    if (double.TryParse(_replaceStr, out double val))
                                        _replaceStr = val.ToString(expression.Format);*/

                                matchedValue = match.Trim('$', '{', '}');
                                string format = "";
                                string[] part = matchedValue.Split(':');
                                if (part.Length > 1)
                                    format = part[1];
                                var rpnExp = RpnTools.ToRpnExpression(part[0]);
                                _replaceStr = RpnTools.CalcRpnStack(rpnExp,_ortdpWrapper).ToString(format);
                                rpnExp.Clear();
                                if (_rawFormat.ToString().Contains("\n") || _rawFormat.ToString().Contains("\r"))
                                {
                                    _rawFormat.Replace("\n", "\n");
                                    _rawFormat.Replace("\r", "\r");
                                }

                                _rawFormat.Replace(match, _replaceStr);
                            }
                            catch (NullReferenceException)
                            {
                            }
                            catch (FormatException)
                            {
                                _rawFormat.Append("Invalid Format");
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                            }

                            _rawFormat.Append("\0\0");
                        }

                    if (boolResult.Length > 0)
                        foreach (string match in boolResult)
                        {
                            try
                            {
                                matchedValue = match;
                                IfExpression expression = new IfExpression(matchedValue, _ortdpWrapper);
                                _replaceStr = expression.GetProcessedValue().ToString();
                                if (_rawFormat.ToString().Contains("\n") || _rawFormat.ToString().Contains("\r"))
                                {
                                    _rawFormat.Replace("\n", "\n");
                                    _rawFormat.Replace("\r", "\r");
                                }

                                _rawFormat.Replace(match, _replaceStr);
                            }
                            catch (NullReferenceException)
                            {
                                _rawFormat.Append("");
                            }

                            _rawFormat.Append("\0\0");

                        }
                }
                catch (Exception x)
                {
                    System.IO.File.AppendAllText("Ex.txt", x.ToString());
                }

                byte[] bytes = Encoding.GetEncoding(Setting.Encoding).GetBytes(_rawFormat.ToString() + "\0\0");
                try
                {
                    _outputMapStream.Write(bytes, 0, bytes.Length);
                }
                catch (NotSupportedException e)
                {
                    IO.CurrentIO.WriteColor(e.ToString(),ConsoleColor.Red);
                }

                
            }

        }
    }
}