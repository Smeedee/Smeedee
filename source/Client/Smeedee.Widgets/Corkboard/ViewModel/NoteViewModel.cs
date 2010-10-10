using System;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.Corkboard.ViewModel
{
    public partial class NoteViewModel : ViewModelBase
    {
        private static Random random = new Random(DateTime.Now.Millisecond);
        public event Move Move;
        public event Delete DeleteNote;

        public virtual int RandomAngle
        {
            get
            {
                return random.Next(-7, 7);
            }
        }

        public void OnDelete()
        {
            if (DeleteNote != null)
                DeleteNote(this, new EventArgs());
        }

        public void OnMoveUp()
        {
            if (Move != null)
                Move(this, new MoveArgs {MoveDirection = MoveArgs.Direction.UP});
        }

        public void OnMoveDown()
        {
            if (Move != null)
                Move(this, new MoveArgs {MoveDirection = MoveArgs.Direction.DOWN});  
        }
    }

    public delegate void Delete(object sender, EventArgs args);
    public delegate void Move(object sender, MoveArgs args);

    public class MoveArgs
    {
        public Direction MoveDirection { get; set; }

        public enum Direction
        {
            UP,
            DOWN
        }
    }
}