using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoTicketSystem
{
    public class OperationTicketHistoryGrid : MonoBehaviour
    {

        public VerticalLayoutGroup grid;//列表
        public OperationTicketHistoryItem itemPrefab;

        public InputField InputFieldPage;//输入框，表示当前第几页，或表示要跳到第几页
        public Button previousPageBtn;//上一页
        public Button nextPageBtn;//下一页
        public Text txtPageCount;//总页数Text
                                 //private List<HistoryPersonsSearchUIItem> selectPersonnelList;//当前显示的列表集合
        private int currentPageNum;//当前所在页
        private int pageCount;//总页数
        private int showCount = 10;//每页显示人员的个数

        private List<OperationTicketHistory> searchList;//当前搜索出来的工作票历史记录

        // Use this for initialization
        void Start()
        {
            previousPageBtn.onClick.AddListener(PreviousPageBtn_OnClick);
            nextPageBtn.onClick.AddListener(NextPageBtn_OnClick);
            InputFieldPage.onEndEdit.AddListener(InputFieldPage_OnEndEdit);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 搜索
        /// </summary>
        public void Search()
        {
            currentPageNum = 0;
            if (TwoTicketHistoryUI_N.Instance.searchInput.text == "")
            {
                searchList = TwoTicketHistoryUI_N.Instance.operationTicketHistoryList;
            }
            else
            {
                searchList = TwoTicketHistoryUI_N.Instance.operationTicketHistoryList.FindAll((item) => Contains(item));
            }

            //if (workTicketHistoryList == null) return;
            pageCount = searchList.Count / showCount;
            if (searchList.Count % showCount > 0)
            {
                pageCount += 1;
            }

            txtPageCount.text = pageCount.ToString();
            InputFieldPage.text = (currentPageNum + 1).ToString();
            CreateGrid();
        }

        /// <summary>
        /// 创建人员列表
        /// </summary>
        public void CreateGrid()
        {
            ClearItems();
            InputFieldPage.text = (currentPageNum + 1).ToString();
            int startIndex = currentPageNum * showCount;
            int num = showCount;
            if (startIndex + num > searchList.Count)
            {
                num = searchList.Count - startIndex;
            }
            if (searchList.Count == 0) return;
            List<OperationTicketHistory> workTicketHistoryT = searchList.GetRange(startIndex, num);

            foreach (OperationTicketHistory w in workTicketHistoryT)
            {
                OperationTicketHistoryItem item = CreatePersonItem(w);
                item.gameObject.SetActive(true);

            }

            SetPreviousAndNextPageBtn();
        }

        /// <summary>
        /// 创建人员列表项
        /// </summary>
        public OperationTicketHistoryItem CreatePersonItem(OperationTicketHistory w)
        {
            OperationTicketHistoryItem item = Instantiate(itemPrefab);
            item.Init(w);
            item.transform.SetParent(grid.transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            //item.gameObject.SetActive(true);
            return item;
        }

        /// <summary>
        /// 工作票筛选筛选
        /// </summary>
        public bool Contains(OperationTicketHistory operationTicketT)
        {
            if (WorkTicketContainsNO(operationTicketT)) return true;
            if (WorkTicketContainsPerson(operationTicketT)) return true;
            return false;
        }

        /// <summary>
        /// 筛选根据工作票编号
        /// </summary>
        public bool WorkTicketContainsNO(OperationTicketHistory operationTicketT)
        {
            if (operationTicketT.No.ToLower().Contains(TwoTicketHistoryUI_N.Instance.searchInput.text.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 筛选根据工作票负责人
        /// </summary>
        public bool WorkTicketContainsPerson(OperationTicketHistory operationTicketT)
        {
            if (operationTicketT.Guardian.ToLower().Contains(TwoTicketHistoryUI_N.Instance.searchInput.text.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 清除列表
        /// </summary>
        public void ClearItems()
        {
            int n = grid.transform.childCount;
            for (int i = n - 1; i >= 0; i--)
            {
                GameObject o = grid.transform.GetChild(i).gameObject;
                //HistoryPersonsSearchUIItem itemT = o.GetComponent<HistoryPersonsSearchUIItem>();
                //if (selectPersonnelList.Contains(itemT))
                //{
                //    o.SetActive(false);
                //    continue;
                //}
                DestroyImmediate(o);
            }
        }

        /// <summary>
        /// 前一页的按钮触发事件
        /// </summary>
        public void PreviousPageBtn_OnClick()
        {
            Debug.Log("PreviousPageBtn_OnClick!");
            currentPageNum = currentPageNum - 1;
            //int currentPageNumT = currentPageNum;
            //SetPreviousAndNextPageBtn();
            CreateGrid();
        }

        /// <summary>
        /// 下一页的按钮触发事件
        /// </summary>
        public void NextPageBtn_OnClick()
        {
            Debug.Log("NextPageBtn_OnClick!");
            currentPageNum = currentPageNum + 1;
            //SetPreviousAndNextPageBtn();
            CreateGrid();
        }

        /// <summary>
        /// 设置前一页和后一页是否开启交互，即interactable
        /// </summary>
        public void SetPreviousAndNextPageBtn()
        {
            if (currentPageNum <= 0)
            {
                currentPageNum = 0;
                previousPageBtn.interactable = false;
            }
            else
            {
                previousPageBtn.interactable = true;
            }

            if (currentPageNum >= pageCount - 1)
            {
                currentPageNum = pageCount - 1;
                nextPageBtn.interactable = false;
            }
            else
            {
                nextPageBtn.interactable = true;
            }
        }

        /// <summary>
        /// 输入页数文本框编辑结束触发事件
        /// </summary>
        /// <param name="txt"></param>
        public void InputFieldPage_OnEndEdit(string txt)
        {
            Debug.Log("InputFieldPage_OnEndEdit!");
        }
    }
}
