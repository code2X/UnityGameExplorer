using ImGuiNET;

namespace DotInsideNode
{
    public interface ITMenuView<T>
    {
        void DrawMenuView(T tObj, out bool onEvent);
    }

    class TEnumMenuView<T,EItemEvent>: ITMenuView<T> where T : dnObject where EItemEvent : System.Enum
    {
        //Event
        public enum EMenuEvent
        {
            Select
        }
        public delegate void TMenuAction(EMenuEvent eEvent, EItemEvent eEnum, T obj);
        public event TMenuAction OnMenuEvent;

        public virtual void DrawMenuView(T tObj,out bool onEvent)
        {
            bool menuClicked = false;
            ImGuiEx.PopupContextItemView(() =>
            {
                menuClicked = DrawMenu(tObj);
            });
            onEvent = menuClicked;
        }

        protected virtual bool DrawMenu(T tObj)
        {
            foreach(var item in GetEnumArray())
            {
                string text = item.ToString().Replace("_", " ");
                if (ImGui.Selectable(text))
                {
                    NotifyMenuEvent(EMenuEvent.Select, (EItemEvent)item, tObj);
                    return true;
                }
            }

            return false;
        }

        protected virtual System.Array GetEnumArray()
        {
            return System.Enum.GetValues(typeof(EItemEvent));
        }

        protected virtual void NotifyMenuEvent(EMenuEvent eMenu, EItemEvent eItem, T obj)
        {
            OnMenuEvent?.Invoke(eMenu, eItem, obj);
        }

    }
}
