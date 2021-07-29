using UnityEngine;

namespace ABXY.Layers.Editor.ThirdParty.Xnode
{
    public class NonBlockingMenu
    {
        public delegate void MenuFunction();

        public delegate void MenuFunction2(object userData);

        public bool allowDuplicateNames;

        private MenuItemDef root;

        public NonBlockingMenu()
        {
            root = new MenuItemDef( null, allowDuplicateNames, false);
        }

        public void AddDisabledItem(GUIContent content)
        {
            MenuItemDef menuItem = new MenuItemDef(content, allowDuplicateNames);
            menuItem.disabled = true;
            root.AddItem(menuItem);
        }

        public void AddItem(GUIContent content, bool on, MenuFunction func, bool searchable = false)
        {
            MenuItemDef menuItem = new MenuItemDef(content, allowDuplicateNames,searchable);
            menuItem.menuFunction = func;
            root.AddItem(menuItem);
        }

        public void AddItem(GUIContent content, bool on, MenuFunction2 func, object data, bool searchable = false)
        {
            MenuItemDef menuItem = new MenuItemDef(content, allowDuplicateNames,searchable);
            menuItem.menuFunction2 = func;
            menuItem.userData = data;
            root.AddItem(menuItem);
        }

        public void AddSeparator(string path)
        {
            root.AddSeparator(path);
        }
        public void DropDown(Rect position)
        {
            NonBlockingMenuWindow.DropDown(position, root);
        }

        public int GetItemCount()
        {
            return 0;
        }

        public void ShowAsContext()
        {

        }

    
    }
}
