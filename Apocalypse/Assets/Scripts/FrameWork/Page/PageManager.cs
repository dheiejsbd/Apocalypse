using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FrameWork.Page
{
    public abstract class PageManager
    {
        static Dictionary<int, IPage> Pages;
        static IPage ActivePage;
        BlackBoard blackBoard;

        #region Event
        public virtual void Initialize()
        {
            Pages = new Dictionary<int, IPage>();
            blackBoard = new BlackBoard();
        }
        public void Update()
        {
            Debug.Log("ActivePageID : " + ActivePage.ID);

            ActivePage?.Update();
        }
        public void LateUpdate()
        {
            ActivePage?.LateUpdate();
        }

        public virtual void Terminate()
        {
            foreach (var item in Pages)
            {
                item.Value.Terminate();
            }
            Pages.Clear();
            Pages = null;
        }
        #endregion

        #region Change
        public void TryChangePage(int NextPageID)
        {
            if (!HasKey(NextPageID))
            {
                Debug.LogError("Fail ChangePage - Fail PageID : " + NextPageID);
            }
            if (ActivePage != null)
            {
                ActivePage.Exit();
                ChangePage(NextPageID);
            }
        }
        void ChangePage(int NextPageID)
        {
            Debug.Log("ChangePage - PageID : " + NextPageID);
            ActivePage = Pages[(int)NextPageID];
            ActivePage.Prepare();
            ActivePage.Enter();
        }
        #endregion

        #region ADD REMOVE
        public void TryAddPage(IPage page)
        {
            if (!HasPage(page))
            {
                Debug.Log("Succsess Add Page - PageID : " + page.ID);
                AddPage(page);
            }
            else
            {
                Debug.LogError("Fail Add Page - PageID : " + page.ID);
            }

        }
        private void AddPage(IPage page)
        {
            Pages.Add(page.ID, page);
            page.Initialize(blackBoard);
        }

        public void TryRemovePage(IPage page)
        {
            if (HasPage(page))
            {
                Debug.Log("Success RemovePage - PageID - " + page.ID);
                RemovePage(page);
            }
            else
            {
                Debug.LogError("Fail RemovePage - PageID : " + page.ID);
            }
        }
        void RemovePage(IPage page)
        {
            Pages.Remove(page.ID);
        }
        #endregion

        #region Confirm
        private bool HasPage(IPage page)
        {
            return Pages.ContainsKey(page.ID);
        }
        private bool HasKey(int ID)
        {
            return Pages.ContainsKey(ID);
        }
        #endregion
    }
}