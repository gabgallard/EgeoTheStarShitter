using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace ABXY.Layers.Editor.ThirdParty.Xnode
{
    public class NonBlockingMenuWindow : EditorWindow
    {
        private MenuItemDef menuItems;

        float leftMargin = 30;

        float rightMargin = 50;

        float lineHeight = 22;

        float separatorHeight = 11;

        private NonBlockingMenuWindow currentlyShowingSubmenu;

        private static Texture2D _expandIcon;

        private SearchField searchField;
        private string searchTerm = "";

        private Vector2 scrollPosition = Vector2.zero;

        private static Texture2D expandIcon
        {
            get
            {
                if (_expandIcon == null)
                    _expandIcon = Resources.Load<Texture2D>("Symphony/Menu Expand");
                return _expandIcon;
            }
        }

        private GUIStyle labelStyle;


    
        private static List<NonBlockingMenuWindow> currentDropDowns = new List<NonBlockingMenuWindow>();

        private int depth = 0;

        private PopupLocationWrapper[] popupLocationPriority = new PopupLocationWrapper[] { PopupLocationWrapper.Right, PopupLocationWrapper.Left, PopupLocationWrapper.Below, PopupLocationWrapper.Above };

        Rect displayLocation;

        public static NonBlockingMenuWindow DropDown(Rect position, MenuItemDef menuItems)
        {
            return DropDown(position, menuItems, true);
        }

        private static NonBlockingMenuWindow DropDown(Rect position, MenuItemDef menuItems, bool overwriteMenuCache)
        {
            NonBlockingMenuWindow window = CreateInstance<NonBlockingMenuWindow>();
            position = GUIUtility.GUIToScreenRect(position);
            position.x = position.x + 1;
            position.y = position.y + 1;

            window.searchField = new SearchField();

            window.menuItems = menuItems;

            window.minSize = new Vector2(0, 0);
            window.maxSize = new Vector2(500, 2048);

            window.labelStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label);
            window.labelStyle.alignment = TextAnchor.MiddleLeft;
            window.ShowAsDropDown(position, new Vector2(window.CalcMaxWidth(), window.CalcHeight()));

            //Debug.Log("Made Window");

            //window.ShowPopup();

            if (overwriteMenuCache)
            {
                currentDropDowns.Clear();
                currentDropDowns.Add(window);

                int menuDepth = CalculateMenuDepth(menuItems);

                for (int index = 0; index < menuDepth; index++)
                    currentDropDowns.Add(null);
            }
            //window.minSize = new Vector2(0, 0);
            //window.maxSize = new Vector2(500, 500);
            Vector2 size = new Vector2(window.CalcMaxWidth(), window.CalcHeight());
            window.displayLocation = window.position = PopupLocationHelperWrapper.GetDropDownRect(position, size, window.popupLocationPriority);

            return window;
        }


        private void SetMenuItems(Rect position, MenuItemDef menuItems)
        {

            position = GUIUtility.GUIToScreenRect(position);
            position.x = position.x + 1;
            position.y = position.y + 1;
            this.menuItems = menuItems;


            labelStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label);
            labelStyle.alignment = TextAnchor.MiddleLeft;
            minSize = new Vector2(0, 0);
            maxSize = new Vector2(500, 2048);

            Vector2 size = new Vector2(CalcMaxWidth(), CalcHeight());

            displayLocation = this.position = PopupLocationHelperWrapper.GetDropDownRect(position, size, popupLocationPriority);
            //ShowAsDropDown(position, new Vector2(CalcMaxWidth(), CalcHeight()));
        }

        private void OnEnable()
        {

        }

        private void SetPositionFromRect(Rect position)
        {

        }

        private void Hide()
        {
            minSize = new Vector2(0, 0);
            menuItems = null;
            displayLocation = this.position = position = new Rect(0, 10000, 0, 0);

            for (int index = depth+1; index < currentDropDowns.Count; index++)
            {
                currentDropDowns[index]?.Hide();
            }
        }

        private static int CalculateMenuDepth(MenuItemDef menuItems)
        {
            int largestChildDepth = 0;
            foreach(MenuItemDef menuItem in menuItems.subItems)
            {
                int childDepth = CalculateMenuDepth(menuItem);
                if (largestChildDepth < childDepth)
                    largestChildDepth = childDepth;
            }
            if (menuItems.subItems.Count > 0)
                largestChildDepth++;

            return largestChildDepth;
        }


        private void OnGUI()
        {
            minSize = new Vector2(0, 0);
            maxSize = new Vector2(500, 2048);

            position = displayLocation;



            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color32(204, 204, 204, 255));
            EditorGUI.DrawRect(new Rect(1, 1, position.width-2, position.height-2), new Color32(242, 242, 242, 255));
            

            if (menuItems == null)
                return;

            float headerHeight = 0;

            if (menuItems.hasSearchableItems && depth == 0) {
                Rect searchRect = new Rect(2f*EditorGUIUtility.standardVerticalSpacing, 2f * EditorGUIUtility.standardVerticalSpacing, 
                    position.width - (EditorGUIUtility.standardVerticalSpacing*4f), EditorGUIUtility.singleLineHeight);
                searchTerm = searchField.OnGUI(searchRect, searchTerm);
                searchField.SetFocus();
                headerHeight += EditorGUIUtility.singleLineHeight + 4f*EditorGUIUtility.standardVerticalSpacing;
            }

            bool doEvents = EditorWindow.mouseOverWindow  == this ;

            List<MenuItemDef> itemsToDraw = string.IsNullOrEmpty(searchTerm) ? menuItems.subItems : menuItems.GetSearchItems(searchTerm);

            Rect scrollRect = new Rect(0, headerHeight, position.width, position.height - headerHeight);
            Rect viewRect = new Rect(Vector2.zero, new Vector2(position.width-20, CalcContentHeight()));

            bool scrollbarIsShowing = viewRect.height > scrollRect.height;

            scrollPosition = GUI.BeginScrollView(scrollRect, scrollPosition, viewRect);

            float lastYPosition = 0;
            foreach (MenuItemDef menuItem in itemsToDraw)
            {

                float width = scrollbarIsShowing ? position.width - 20 : position.width;

                if (menuItem.isSeparator)
                {
                    Rect separatorPosition = new Rect(0, lastYPosition, width, separatorHeight);
                    DrawSeparator(separatorPosition);
                    lastYPosition += separatorHeight;
                }
                else
                {
                    Rect buttonPosition = new Rect(0, lastYPosition, width, lineHeight);
                    DrawAsMenuItem(buttonPosition, menuItem, doEvents);


                    lastYPosition += lineHeight;
                }
            }

            GUI.EndScrollView();

            Repaint();

        }

        private void DrawSeparator(Rect position)
        {
            float leftMargin = 46;
            float rightMargin = 3;
            float yOffset = (position.height - 1f) / 2f;
            EditorGUI.DrawRect(new Rect(leftMargin,position.y + yOffset, position.width - leftMargin - rightMargin, 1) , new Color32(215,215,215,255));
        }

        private void DrawAsMenuItem(Rect buttonPosition, MenuItemDef menuItem, bool doEvents)
        {
            if ( buttonPosition.Contains(Event.current.mousePosition))
            {
                if (doEvents)
                    EditorGUI.DrawRect(buttonPosition, new Color32(145, 201, 247, 255));
                if (currentlyShowingSubmenu != null && menuItem != currentlyShowingSubmenu.menuItems && doEvents)
                {
                    currentlyShowingSubmenu.Hide();
                    currentlyShowingSubmenu = null;
                }

                if (currentlyShowingSubmenu == null && menuItem.isFolder && !menuItem.disabled && doEvents)
                {
                    //Rect drawRect = new Rect(buttonPosition.x + buttonPosition.width, buttonPosition.y, 0, 0);
                    NonBlockingMenuWindow newWindow = GetWindowAtDepth(depth + 1, buttonPosition, menuItem);
                    currentlyShowingSubmenu = newWindow;
                }
                //    currentlyShowingSubmenu = DropDown(new Rect(buttonPosition.x + buttonPosition.width, buttonPosition.y, 0, 0), menuItem);
            
                if ((menuItem.menuFunction != null || menuItem.menuFunction2 != null) && Event.current.type == EventType.MouseDown && Event.current.button == 0 && !menuItem.disabled && doEvents)
                {
                    menuItem.menuFunction?.Invoke();
                    menuItem.menuFunction2?.Invoke(menuItem.userData);
                    CloseAll();
                }
                if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout && Event.current.type != EventType.ScrollWheel)
                    Event.current.Use();
            }

            EditorGUI.BeginDisabledGroup(menuItem.disabled);
            Rect labelPosition = new Rect(leftMargin, buttonPosition.y - 1, position.width - leftMargin, lineHeight);
            EditorGUI.LabelField(labelPosition, menuItem.content, labelStyle);
            EditorGUI.EndDisabledGroup();

            if (menuItem.isFolder)
            {
                float expandButtonSize = 13;
                float yPosition = (lineHeight - expandButtonSize) / 2f;
                Rect expandIconRect = new Rect(buttonPosition.width - expandButtonSize - 12, buttonPosition.y + yPosition, expandButtonSize, expandButtonSize);
                GUI.Label(expandIconRect, expandIcon);
            }

        }

        private static NonBlockingMenuWindow GetWindowAtDepth(int depth, Rect position, MenuItemDef menuItems)
        {
            if (depth < 0 || currentDropDowns.Count <= depth)
                return null;

            NonBlockingMenuWindow result = null;
            if (currentDropDowns[depth] == null)
            {
                currentDropDowns[depth] = DropDown(position, menuItems, false);
            }
            else
                currentDropDowns[depth].SetMenuItems(position, menuItems);
            result = currentDropDowns[depth];
            result.depth = depth;
            return result;
        }


        private float CalcMaxWidth()
        {
            float maxWidth = EditorGUIUtility.fieldWidth;
            foreach (MenuItemDef menuItem in menuItems.subItems)
            {
                if (menuItem.isSeparator)
                    continue;
                float size = labelStyle.CalcSize(menuItem.content).x;
                if (maxWidth < size)
                    maxWidth = size;
            }
            return maxWidth + leftMargin + rightMargin;
        }

        private float CalcHeight()
        {
            float height = 0;

            if (menuItems.hasSearchableItems && depth == 0)
                height += EditorGUIUtility.singleLineHeight + 4f*EditorGUIUtility.standardVerticalSpacing;

            height += CalcContentHeight();
            
            return height;
        }

        private float CalcContentHeight()
        {
            List<MenuItemDef> itemsToDraw = string.IsNullOrEmpty(searchTerm) ? menuItems.subItems : menuItems.GetSearchItems(searchTerm);

            float height = 0;
            foreach (MenuItemDef menuItem in itemsToDraw)
            {
                if (menuItem.isSeparator)
                    height += separatorHeight;
                else
                    height += lineHeight;
            }
            return height;
        }

        private void OnDestroy()
        {
        
        }


        private void CloseAll()
        {
            for (int index = 0; index < currentDropDowns.Count; index++)
                currentDropDowns[index]?.Close();
            currentDropDowns.Clear();
            this.Close();
        }

        public override string ToString()
        {
            return name + depth;
        }
    }
}
