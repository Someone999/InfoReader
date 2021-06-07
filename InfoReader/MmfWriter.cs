using System.Globalization;
using System.Threading;
using InfoReaderPlugin.MemoryMapWriter.ExpressionTools;

namespace InfoReaderPlugin
{
    using InfoReaderPlugin.MemoryMapWriter;
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    partial class InfoReader
    {
        StringBuilder _rawFormat;
        string _replaceStr = "";
        string _matchedValue;
        private readonly System.IO.MemoryMappedFiles.MemoryMappedFile _outputInfoMap;
        private System.IO.Stream _outputMapStream;
        void RefreshMmf()
        {
            while (true)
            {
                
                Thread.Sleep(5);
                try
                {
                    if (_rawFormat?.ToString() != _fileFormat)
                        _rawFormat = new StringBuilder(_fileFormat ?? "");
                    Regex varPattern = new Regex(@"\$\{[^,\{\}]*(:[\w\.]*)?\}"); //
                    Regex boolPattern = new Regex(@"\$if\{[^\{\}]*[\w><=!&\|\+\-\*/\(\)]*,[^{}]*,[^{}]*\}");
                    MatchCollection varMatches = varPattern.Matches(_rawFormat.ToString());
                    MatchCollection boolMatches = boolPattern.Matches(_rawFormat.ToString());
                    if (varMatches.Count > 0)
                        foreach (Match match in varMatches)
                        {
                            try
                            {
                                _matchedValue = match.Value;
                                VariableExpression expression = new VariableExpression(_matchedValue, _ortdpWrapper);

                                _replaceStr = ExpressionTools.CalcRpnExpression(
                                    ExpressionTools.ConvertToRpnExpression(expression.NoFormatExpression), _ortdpWrapper);
                                if (expression.HasFormat)
                                    if (double.TryParse(_replaceStr, out double val))
                                        _replaceStr = val.ToString(expression.Format);
                                if (_rawFormat.ToString().Contains("\n") || _rawFormat.ToString().Contains("\r"))
                                {
                                    _rawFormat.Replace("\n", "\n");
                                    _rawFormat.Replace("\r", "\r");
                                }

                                _rawFormat.Replace(match.Value, _replaceStr);
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

                    if (boolMatches.Count > 0)
                        foreach (Match match in boolMatches)
                        {
                            try
                            {
                                _matchedValue = match.Value;
                                IfExpression expression = new IfExpression(_matchedValue, _ortdpWrapper);
                                _replaceStr = expression.GetProcessedValue().ToString();
                                if (_rawFormat.ToString().Contains("\n") || _rawFormat.ToString().Contains("\r"))
                                {
                                    _rawFormat.Replace("\n", "\n");
                                    _rawFormat.Replace("\r", "\r");
                                }

                                _rawFormat.Replace(match.Value, _replaceStr);
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
                _outputMapStream = _outputInfoMap.CreateViewStream();
                _outputMapStream.Write(bytes, 0, bytes.Length);
            }

        }
    }
}