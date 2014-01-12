namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    public class NameTransformer : BindableCollection<NameTransformer.Rule>
    {
        private bool useEagerRuleSelection = true;

        public void AddRule(string replacePattern, string replaceValue, string globalFilterPattern = null)
        {
            this.AddRule(replacePattern, new string[] { replaceValue }, globalFilterPattern);
        }

        public void AddRule(string replacePattern, IEnumerable<string> replaceValueList, string globalFilterPattern = null)
        {
            Rule item = new Rule {
                ReplacePattern = replacePattern,
                ReplacementValues = replaceValueList,
                GlobalFilterPattern = globalFilterPattern
            };
            base.Add(item);
        }

        public IEnumerable<string> Transform(string source)
        {
            return this.Transform(source, r => r);
        }

        public IEnumerable<string> Transform(string source, System.Func<string, string> getReplaceString)
        {
            List<string> list = new List<string>();
            IEnumerable<Rule> enumerable = this.Reverse<Rule>();
            using (IEnumerator<Rule> enumerator = enumerable.GetEnumerator())
            {
                System.Func<string, string> func = null;
                Rule rule;
                while (enumerator.MoveNext())
                {
                    rule = enumerator.Current;
                    if ((string.IsNullOrEmpty(rule.GlobalFilterPattern) || Regex.IsMatch(source, rule.GlobalFilterPattern)) && Regex.IsMatch(source, rule.ReplacePattern))
                    {
                        if (func == null)
                        {
                            func = repString => Regex.Replace(source, rule.ReplacePattern, repString);
                        }
                        list.AddRange(Enumerable.Select<string, string>(Enumerable.Select<string, string>(rule.ReplacementValues, getReplaceString), func));
                        if (!this.useEagerRuleSelection)
                        {
                            return list;
                        }
                    }
                }
            }
            return list;
        }

        public bool UseEagerRuleSelection
        {
            get
            {
                return this.useEagerRuleSelection;
            }
            set
            {
                this.useEagerRuleSelection = value;
            }
        }

        public class Rule
        {
            public string GlobalFilterPattern;
            public IEnumerable<string> ReplacementValues;
            public string ReplacePattern;
        }
    }
}

