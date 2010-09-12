using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.Widget.Corkboard.ViewModels
{
    interface INoteCollection
    {
        void AddNote(NoteViewModel note);
    }
}
