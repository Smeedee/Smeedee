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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Smeedee.Client.Framework.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widget.SourceControl.ViewModels
{
    public class LatestCommitsViewModel : AbstractViewModel
    {
        public DelegateCommand SaveSettings { get; set; }
        public DelegateCommand ReloadSettings { get; set; }
        public DelegateCommand AddWordAndColorSettings { get; set; }
        

        public const int NUMBER_OF_COMMITS_DEFAULT = 8;
        public const bool BLINK_WHEN_NO_COMMENT_DEFAULT = false;

        public LatestCommitsViewModel(Client.Framework.ViewModel.Widget widget)
        {
            Changesets = new ObservableCollection<ChangesetViewModel>();
            KeywordList = new ObservableCollection<KeywordColorPair>();

            BlinkWhenNoComment = true;

            SaveSettings = new DelegateCommand();
            ReloadSettings = new DelegateCommand();
            AddWordAndColorSettings = new DelegateCommand();

            ReloadSettings.ExecuteDelegate += Reset;
            widget.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "IsInSettingsMode") 
                        Reset();
                };
            
            NumberOfCommits = NUMBER_OF_COMMITS_DEFAULT;
            BlinkWhenNoComment = BLINK_WHEN_NO_COMMENT_DEFAULT;
            SetResetPoint();
        }

        public ObservableCollection<ChangesetViewModel> Changesets { get; private set; }
        
        private int numberOfCommits;
        private int numberOfCommitsResetPoint;
        public int NumberOfCommits
        {
            get { return numberOfCommits; }
            set
            {
                if (ReturnNumberOfCommitsIfValid(value) != numberOfCommits) 
                {
                    numberOfCommits = value;
                    TriggerPropertyChanged<LatestCommitsViewModel>(t => t.NumberOfCommits);
                }
            }
        }

        private static int ReturnNumberOfCommitsIfValid(int value)
        {
            if ( value > 0)
            {
                return value;
            }
            throw new Exception("Must be positive number");
        }

        private bool blinkWhenNoComment;
        private bool blinkWhenNoCommentResetPoint;
        public bool BlinkWhenNoComment
        {
            get { return blinkWhenNoComment; }
            set
            {
                if (value != blinkWhenNoComment)
                {
                    blinkWhenNoComment = value;
                    TriggerPropertyChanged<LatestCommitsViewModel>(t => t.BlinkWhenNoComment);
                }
            }
        }

        private bool isSaving;

        public bool IsSaving
        {
            get { return isSaving; }
            set
            {
                if (value != isSaving)
                {
                    isSaving = value;
                    TriggerPropertyChanged<LatestCommitsViewModel>(t => t.IsSaving);
                }
            }
        }

        public ObservableCollection<KeywordColorPair> KeywordList { get; private set; }

        

        public void SetResetPoint()
        {
            blinkWhenNoCommentResetPoint = BlinkWhenNoComment;
            numberOfCommitsResetPoint = NumberOfCommits;
        }

        public void Reset()
        {
            BlinkWhenNoComment = blinkWhenNoCommentResetPoint;
            NumberOfCommits = numberOfCommitsResetPoint;
        }
    }

    public class KeywordColorPair
    {
        public string Keyword { get; set; }
        public string ColorName { get; set; }
    }

    public class ColorProvider
    {
        public List<string> ColorList
        {
            get
            {
                return new List<string>
                           {
                               "red", "green", "blue"
                           };
            }
        }
    }
}