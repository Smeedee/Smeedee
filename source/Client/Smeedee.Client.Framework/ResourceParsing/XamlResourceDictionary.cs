#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://www.smeedee.org
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace Smeedee.Client.Framework.ResourceParsing
{
    public class XamlResourceDictionary
    {
        private System.Windows.ResourceDictionary internalDictionary;
        private readonly IXamlReader xamlReader;

        public XamlResourceDictionary(string resourcePath, IXamlReader xamlReader)
        {
            this.xamlReader = xamlReader;
            SetupResourceStream(resourcePath);
            CreateDictionary();
        }

        public object this[string resourceKey]
        {
            get { return internalDictionary[resourceKey]; }
        }

        public StreamResourceInfo ResourceStream { get; private set; }

        private void SetupResourceStream(string resourcePath)
        {
            if (Uri.IsWellFormedUriString(resourcePath, UriKind.Relative))
            {
                ResourceStream = Application.GetResourceStream(new Uri(resourcePath, UriKind.Relative));
            }
            else
            {
                throw new UriFormatException("URI is not well formed");
            }
        }

        private void CreateDictionary()
        {
            string xamlCode = string.Empty;

            using (var sr = new StreamReader(ResourceStream.Stream))
            {
                xamlCode = sr.ReadToEnd();
                sr.Close();
            }

            if (!string.IsNullOrEmpty(xamlCode))
            {
                internalDictionary = (System.Windows.ResourceDictionary) xamlReader.Load(xamlCode);
            }
        }
    }
}