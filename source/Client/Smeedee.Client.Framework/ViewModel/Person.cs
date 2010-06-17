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
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;

namespace Smeedee.Client.Framework.ViewModel
{
    public class Person : AbstractViewModel
    {
        private string _email;
        private string _firstname;
        private string _imageUrl;
        private string _middlename;
        private string _surname;
        private string _username;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                TriggerPropertyChanged<Person>(vm => vm.Username);
            }
        }

        public string Name
        {
            get
            {
                string combinedName = Firstname +
                                        (Middlename != null ? " " + Middlename : string.Empty) +
                                        (Surname != null ? " " + Surname : string.Empty);

                if (string.IsNullOrEmpty(combinedName))
                    return Username;
                else
                    return combinedName;
            }
        }

        public virtual string Firstname
        {
            get { return _firstname; }
            set
            {
                _firstname = value;
                TriggerPropertyChanged<Person>(vm => vm.Firstname);
                TriggerPropertyChanged<Person>(vm => vm.Name);
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (value != _email)
                {
                    _email = value;
                    TriggerPropertyChanged<Person>(vm => vm.Email);
                }
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl ?? "~/UserImages/unknown_user.jpg"; }
            set
            {
                if (value != _imageUrl)
                {
                    _imageUrl = value;
                    TriggerPropertyChanged<Person>(vm => vm.ImageUrl);
                }
            }
        }

        public string Middlename
        {
            get { return _middlename; }
            set
            {
                _middlename = value;
                TriggerPropertyChanged<Person>(vm => vm.Middlename);
                TriggerPropertyChanged<Person>(vm => vm.Name);
            }
        }

        public string Surname
        {
            get { return _surname; }
            set
            {
                _surname = value;
                TriggerPropertyChanged<Person>(vm => vm.Surname);
                TriggerPropertyChanged<Person>(vm => vm.Name);
            }
        }
    }

    public class UnknownPerson : Person
    {
        public override string Firstname
        {
            get { return "Unknown"; }
            set { throw new Exception("Property Firstname is read only"); }
        }
    }
}