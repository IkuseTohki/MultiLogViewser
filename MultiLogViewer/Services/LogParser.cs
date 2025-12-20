using MultiLogViewer.Models;
using System.Text.RegularExpressions;
using System.Globalization;
using System;
using System.Collections.Generic;

namespace MultiLogViewer.Services
{
    public class LogParser : ILogParser
    {
        private readonly LogFormatConfig _config;
        private readonly Regex _regex;

        public LogParser(LogFormatConfig config)
        {
            _config = config;
            _regex = new Regex(config.Pattern, RegexOptions.Compiled);
        }

        public LogEntry? Parse(string logLine, string fileName, int lineNumber)
        {
            var match = _regex.Match(logLine);

            if (!match.Success)
            {
                return null;
            }

            var logEntry = new LogEntry
            {
                FileName = fileName,
                LineNumber = lineNumber
            };

            // Timestampのパース
            if (match.Groups["timestamp"].Success)
            {
                var val = ApplyTransforms(match.Groups["timestamp"].Value, "timestamp", _config.FieldTransforms);
                if (DateTime.TryParseExact(val, _config.TimestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timestamp))
                {
                    logEntry.Timestamp = timestamp;
                }
                else
                {
                    logEntry.Timestamp = DateTime.MinValue;
                }
            }
            else
            {
                logEntry.Timestamp = DateTime.MinValue;
            }

            // Messageの取得
            if (match.Groups["message"].Success)
            {
                logEntry.Message = ApplyTransforms(match.Groups["message"].Value, "message", _config.FieldTransforms);
            }

            // その他のキャプチャグループをAdditionalDataに格納
            foreach (Group group in match.Groups)
            {
                if (group.Name != "0" && group.Success &&
                    group.Name != "timestamp" && group.Name != "message")
                {
                    logEntry.AdditionalData[group.Name] = ApplyTransforms(group.Value, group.Name, _config.FieldTransforms);
                }
            }

            // サブパターンの適用
            if (_config.SubPatterns != null)
            {
                foreach (var subPattern in _config.SubPatterns)
                {
                    string sourceValue = string.Empty;
                    if (subPattern.SourceField == "message")
                    {
                        sourceValue = logEntry.Message;
                    }
                    else if (logEntry.AdditionalData.ContainsKey(subPattern.SourceField))
                    {
                        sourceValue = logEntry.AdditionalData[subPattern.SourceField];
                    }

                    if (!string.IsNullOrEmpty(sourceValue))
                    {
                        var subRegex = new Regex(subPattern.Pattern);
                        var subMatch = subRegex.Match(sourceValue);
                        if (subMatch.Success)
                        {
                            foreach (Group group in subMatch.Groups)
                            {
                                if (group.Name != "0" && group.Success)
                                {
                                    logEntry.AdditionalData[group.Name] = ApplyTransforms(group.Value, group.Name, subPattern.FieldTransforms);
                                }
                            }
                        }
                    }
                }
            }

            return logEntry;
        }

        private string ApplyTransforms(string value, string fieldName, List<FieldTransformConfig> transforms)
        {
            if (transforms == null) return value;

            var transform = transforms.Find(t => t.Field == fieldName);
            if (transform == null) return value;

            string result = value;

            // 1. Map による置換
            if (transform.Map != null && transform.Map.TryGetValue(value, out var mappedValue))
            {
                result = mappedValue;
            }

            // 2. Format による整形
            if (!string.IsNullOrEmpty(transform.Format))
            {
                result = transform.Format.Replace("{value}", result);
            }

            return result;
        }
    }
}
