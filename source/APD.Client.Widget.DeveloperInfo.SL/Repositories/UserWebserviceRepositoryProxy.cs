using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using APD.Client.Framework.SL;
using APD.Client.Framework.SL.UserRepositoryService;
using APD.DomainModel.Users;
using APD.DomainModel.Framework;
using System.Collections.Generic;

using User = APD.DomainModel.Users.User;
using UserdbNameSpecification = APD.DomainModel.Users.UserdbNameSpecification;


namespace APD.Client.Widget.DeveloperInfo.SL.Repositories
{
    public class UserWebserviceRepositoryProxy : IRepository<User>
    {
        private UserRepositoryServiceClient serviceClient;
        private static List<User> usersCache = new List<User>();

        public UserWebserviceRepositoryProxy()
        {
            serviceClient = new UserRepositoryServiceClient("CustomBinding_UserRepositoryService");
            serviceClient.Endpoint.Address =
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(serviceClient.Endpoint.Address);

            serviceClient.GetCompleted += (o, e) =>
            {
                var servers = e.Result;
                if (servers.Count() > 0)
                {
                    var tmpUsersCache = new List<User>();            
                    foreach (var user in servers.First().Users)
                    {
                        tmpUsersCache.Add(user);
                    }
                    tmpUsersCache.AddRange(QueryAllDefaultUsers());
                    usersCache = tmpUsersCache;
                }
            };
        }

        private static List<User> QueryAllDefaultUsers()
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
            };

            return retValue;
        }

        public IEnumerable<User> Get(Specification<User> specification)
        {
            serviceClient.GetAsync(new UserdbNameSpecification("default"));
            var retValue = usersCache.Where(u => specification.IsSatisfiedBy(u)).ToList();
            return retValue;
        }
    }
}
