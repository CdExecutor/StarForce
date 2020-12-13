using GameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace StarForce
{
    public class ConfirmForm : MonoBehaviour
    {
        [SerializeField]
        private Text m_title;
        [SerializeField]
        private Text m_content;
        private object m_UserData = null;

        /// <summary>
        /// 确定按钮回调。
        /// </summary>
        public GameFrameworkAction<object> OnClickConfirm
        {
            get;
            set;
        }

        /// <summary>
        /// 取消按钮回调。
        /// </summary>
        public GameFrameworkAction<object> OnClickCancel
        {
            get;
            set;
        }

        private void Start()
        {
        }

        public void OnClickConfirmBtn()
        {
            OnClickConfirm?.Invoke(m_UserData);
        }

        public void OnClickCancelBtn()
        {
            OnClickCancel?.Invoke(m_UserData);
        }

        public void SetConfirUI(DialogParams dialogParams)
        {
            if (m_title != null)
            {
                m_title.text = dialogParams.Title;
            }

            if (m_content != null)
            {
                m_content.text = dialogParams.Message;
            }

            OnClickConfirm = dialogParams.OnClickConfirm;
            OnClickCancel = dialogParams.OnClickCancel;
            m_UserData = dialogParams.UserData;
        }
    }
}
