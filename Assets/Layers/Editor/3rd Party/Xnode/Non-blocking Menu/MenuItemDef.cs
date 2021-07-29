using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Editor.ThirdParty.Xnode
{
    public class MenuItemDef
    {


        //public string path { get; private set; }

        public GUIContent content { get; private set; }

        public List<MenuItemDef> subItems = new List<MenuItemDef>();

        public bool isFolder { get { return subItems.Count != 0; } }


        private bool allowDuplicateNames;

        public bool disabled = false;

        public bool isSeparator = false;

        public NonBlockingMenu.MenuFunction menuFunction;

        public NonBlockingMenu.MenuFunction2 menuFunction2;

        public object userData;

        public bool searchable { get; private set; }

        public bool hasSearchableItems { get; private set; }


        public MenuItemDef(GUIContent content, bool allowDuplicateNames, bool searchable = false)
        {
            //this.path = path;
            this.content = content;
            this.allowDuplicateNames = allowDuplicateNames;
            this.searchable = searchable;
        }


        public void AddItem(MenuItemDef menuItem)
        {
            AddMenuItem(menuItem);
        }



        public void AddSeparator(string path)
        {
            string rootDirectory = GetRootDirectory(path);


            string restOfPath = GetRestOfPath(path);

            bool isFolder = restOfPath != "";

            MenuItemDef preExistingMenuItem = subItems.Find(x => x.content.text == rootDirectory);

            if (isFolder)
            {
                path = restOfPath;
                if (preExistingMenuItem == null)
                {
                    preExistingMenuItem = new MenuItemDef(new GUIContent(rootDirectory), allowDuplicateNames);
                    subItems.Add(preExistingMenuItem);

                }
                preExistingMenuItem.AddSeparator(path);
            }
            else
            {
                MenuItemDef separator = new MenuItemDef(new GUIContent("Separator-" + System.Guid.NewGuid().ToString()), allowDuplicateNames, false);
                separator.isSeparator = true;
                subItems.Add(separator);
            

            }
        }

        private void AddMenuItem (MenuItemDef menuItem)
        {
            if (menuItem.searchable)
                hasSearchableItems = true;

            string rootDirectory = GetRootDirectory(menuItem.content.text);
        
            if (string.IsNullOrEmpty(rootDirectory))
                return;

            string restOfPath = GetRestOfPath(menuItem.content.text);

            bool isFolder = restOfPath != "";

            MenuItemDef preExistingMenuItem = subItems.Find(x => x.content.text == rootDirectory);

            if (isFolder)
            {
                menuItem.content.text = restOfPath;
                if (preExistingMenuItem == null)
                {
                    preExistingMenuItem = new MenuItemDef(new GUIContent(rootDirectory), allowDuplicateNames);
                    subItems.Add(preExistingMenuItem);
                
                }
                preExistingMenuItem.AddItem(menuItem);
            }
            else
            {
                if (menuItem.isSeparator)
                {
                    menuItem.content = new GUIContent("Separator-" + System.Guid.NewGuid().ToString());
                    preExistingMenuItem = null;
                }
                if (allowDuplicateNames || preExistingMenuItem == null)
                {
                    subItems.Add(menuItem);
                }

            }
        }

        public List<MenuItemDef> GetSearchItems(string searchTerm)
        {
            List<MenuItemDef> items = new List<MenuItemDef>();
            searchTerm = searchTerm.ToLower();
            foreach (MenuItemDef item in subItems)
            {
                if (item.isFolder)
                {
                    items.AddRange(item.GetSearchItems(searchTerm));
                } else if (item.content.text.ToLower().Contains(searchTerm))
                    items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Gets the part of the path that's not part of the root directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetRestOfPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";


            int separatorLocation = GetSeparatorIndex(path);
            if (separatorLocation == 0)//removing first slash
            {
                path = path.Substring(1, path.Length - 1);
                separatorLocation = GetSeparatorIndex(path);
            }

            if (separatorLocation == -1)
                return "";

            return path.Substring(separatorLocation + 1, path.Length - separatorLocation- 1);
        }

        private static string GetRootDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            int separatorLocation = GetSeparatorIndex(path);
            if (separatorLocation == 0)//removing first slash
            {
                path = path.Substring(1, path.Length - 1);
                separatorLocation = GetSeparatorIndex(path);
            }

            if (separatorLocation == -1)
                return path;

            return path.Substring(0, separatorLocation);

        }

        private static int GetSeparatorIndex(string path)
        {
            int separatorIndex = -1;
            int backslashLocation = path.IndexOf('\\');
            int forwardslashLocation = path.IndexOf('/');
            if (backslashLocation >= 0)
                separatorIndex = backslashLocation;
            if (forwardslashLocation >= 0 && (forwardslashLocation < backslashLocation || backslashLocation == -1))
                separatorIndex = forwardslashLocation;
            return separatorIndex;
        }
    }
}
