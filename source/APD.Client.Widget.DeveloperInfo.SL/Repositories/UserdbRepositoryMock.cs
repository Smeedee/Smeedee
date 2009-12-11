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
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System.Collections.Generic;
using System.Linq;

using APD.DomainModel.Framework;
using APD.DomainModel.Users;


namespace APD.Client.Widget.DeveloperInfo.SL.Repositories
{
    public class UserdbRepositoryMock : IRepository<User>
    {
        private List<User> users;

        public UserdbRepositoryMock()
        {
            users = new List<User>();
        }

        private List<User> QueryData()
        {
            var retValue = new List<User>
            {
                new User
                {
                    Username = "jaffe",
                    Firstname = "Jan Fredrik",
                    Surname = "Drabløs",
                    Email = "jaffe88@hotmail.com",
                    ImageUrl = "http://files.itslearning.com/data/his/23291/Jan.jpg"
                },
                new User
                {
                    Username = "dnyvik",
                    Firstname = "Daniel",
                    Surname = "Nyvik",
                    Email = "d.nyvik@gmail.com",
                    ImageUrl = "http://farm4.static.flickr.com/3337/3236565707_89ccc74268.jpg"
                },
                new User
                {
                    Username = "joachim",
                    Firstname = "Joachim",
                    Middlename = "Hjelmås",
                    Surname = "Andersen",
                    Email = "numusic@gmail.com",
                    ImageUrl = "http://www.tabloidprodigy.com/wp-content/uploads/2009/05/mrt15.jpg"
                },
                new User
                {
                    Username = "tuxbear",
                    Firstname = "Ole André",
                    Surname = "Johansen",
                    Email = "oleandrejohansen@gmail.com",
                    ImageUrl =
                        "http://drift.idi.ntnu.no/guru/om-gurutjenesten/gurubilder/gurubilderoleandrejohansen.jpg"
                },
                new User
                {
                    Username = "Pete",
                    Firstname = "Peter",
                    Surname = "Samuel",
                    Email = "pjsamuel3@hotmail.com",
                    ImageUrl = "http://sunbulli.com/wp-content/uploads/2007/04/jabba.jpg"
                },
                new User
                {
                    Username = "ole_gunnar",
                    Firstname = "Ole Gunnar",
                    Surname = "Borstad",
                    Email = "ole-gunnar.borstad@capgemini.com ",
                    ImageUrl = "http://sunbulli.com/wp-content/uploads/2007/04/jabba.jpg"
                },
                new User
                {
                    Username = "dagolap",
                    Firstname = "Dag Olav",
                    Surname = "Prestgarden",
                    Email = "dagolav@prestegarden.com ",
                    ImageUrl =
                        "http://photos-a.ak.fbcdn.net/photos-ak-sf2p/v115/55/75/810370108/n810370108_926896_1116.jpg"
                },
                new User
                {
                    Username = "torstn",
                    Firstname = "Torstein",
                    Surname = "Nicolaysen",
                    Email = "tnicolaysen@gmail.com",
                    ImageUrl = "http://pageslap.files.wordpress.com/2009/05/tim-toolman-taylor.jpg"
                },
                new User
                {
                    Username = "jask",
                    Firstname = "Jonas",
                    Surname = "Ask",
                    Email = "jonask84@gmail.com ",
                    ImageUrl =
                        "http://www.facebook.com/profile/pic.php?uid=AAAAAQAQPAZ_5aoRMF4jCXchiZH7pQAAAAlWetCneWed7vkmMlNQoSMN"
                },
                new User
                {
                    Username = "goeran",
                    Firstname = "Gøran",
                    Surname = "Hansen",
                    Email = "mail@goeran.no",
                    ImageUrl =
                        "http://s3.amazonaws.com/twitter_production/profile_images/55894638/IMG_4500_2.JPG"
                },
                new User
                {
                    Username = "GOEran_cp",
                    Firstname = "Gøran",
                    Surname = "Hansen",
                    Email = "mail@goeran.no",
                    ImageUrl =
                        "http://s3.amazonaws.com/twitter_production/profile_images/55894638/IMG_4500_2.JPG"
                },
                new User
                {
                    Username = "esortebe",
                    Firstname = "Eivind",
                    Surname = "Sorteberg",
                    Email = "eivind.sorteberg@gmail.com",
                    ImageUrl = "http://photos-g.ak.fbcdn.net/hphotos-ak-snc1/hs107.snc1/4608_204367355322_639185322_6959214_3069343_n.jpg"
                },
                new User
                {
                    Username = "sorteberg_cp",
                    Firstname = "Eivind",
                    Surname = "Sorteberg",
                    Email = "eivind.sorteberg@gmail.com",
                    ImageUrl = "http://photos-g.ak.fbcdn.net/hphotos-ak-snc1/hs107.snc1/4608_204367355322_639185322_6959214_3069343_n.jpg"
                },
                new User
                {
                    Username = "remi",
                    Firstname = "Remi",
                    Middlename = "E",
                    Surname = "Robberstad",
                    Email = "remirobb@gmail.com",
                    ImageUrl = "http://www.facebook.com/profile/pic.php?uid=AAAAAQAQD1wrbkmlCvxjwAl7f8SDzQAAAApFj8DeQMH1nM2Ao5Lf-BXJ"
                },
                new User
                {
                    Username = "runegri",
                    Firstname = "Rune Andreas",
                    Surname = "Grimstad",
                    Email = "rag@rag.no",
                    ImageUrl = "http://www.facebook.com/profile/pic.php?uid=AAAAAQAQD1wrbkmlCvxjwAl7f8SDzQAAAApFj8DeQMH1nM2Ao5Lf-BXJ"
                },
                new User
                {
                    Username = "heine",
                    Firstname = "Heine",
                    Surname = "Kolltveit",
                    Email = "heine@kolltveit.net",
                    ImageUrl = "http://profile.ak.fbcdn.net/v225/1749/121/n584630424_5301.jpg"
                },
                new User
                {
                    Username = "default",
                    Firstname = "Joe Doe",
                    Email = "joe@doe.no",
                    ImageUrl =
                        "UserImages/default_user.jpg"
                },
                new User
                {
                    Username = "system",
                    Firstname = "System user",
                    Email = "",
                    ImageUrl = "UserImages/system_user.jpg"
                },
                new User
                {
                    Username = "unknown",
                    Firstname = "Unknown user",
                    Email = "",
                    ImageUrl = "UserImages/unknown_user.jpg"
                }
            };

            return retValue;
        }

        #region IRepository<User> Members

        public IEnumerable<User> Get(Specification<User> specification)
        {
            IEnumerable<User> q = QueryData().Where(u => specification.IsSatisfiedBy(u));

            if (specification is AllSpecification<User>)
                return q.ToList();

            bool usersFound = q.Count() > 0;

            if (usersFound)
            {
                return new List<User>(){ q.FirstOrDefault() };
            }
            else
            {
                return new List<User>()
                {
                    new User
                    {
                        Username =
                            ( specification is UserByUsername )
                                ? ( (UserByUsername) specification ).Username
                                : User.unknownUser.Username,
                        Firstname =
                            ( specification is UserByUsername )
                                ? ( (UserByUsername) specification ).Username
                                : User.unknownUser.Firstname,
                        Surname = string.Empty,
                        ImageUrl = User.unknownUser.ImageUrl
                    }
                };
            }

        }

        #endregion
    }
}