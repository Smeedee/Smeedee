using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Smeedee.Client.Framework.SL.ValidationHandler
{
    public class ValidationHandler
    {
        private Dictionary<string, string> BrokenRules { get; set; }
        public ValidationHandler()
        {
            BrokenRules = new Dictionary<string, string>();
        }
        public string this[string property]
        {
            get
            {
                return this.BrokenRules[property];
            }
        }
        public bool BrokenRuleExists(string property)
        {
            return BrokenRules.ContainsKey(property);
        }
        public bool ValidateRule(string property, string message, Func<bool> ruleCheck)
        {
            if (!ruleCheck())
            {
                this.BrokenRules.Add(property, message);
                return false;
            }
            else
            {
                RemoveBrokenRule(property);
                return true;
            }
        }
        public void RemoveBrokenRule(string property)
        {
            if (this.BrokenRules.ContainsKey(property))
            {
                this.BrokenRules.Remove(property);
            }
        }
    }
}
