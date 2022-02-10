using UnityEngine;
using UnityEngine.UI;

namespace HQFPSTemplate.UserInterface
{
    public class UI_WheelSlot : UI_ItemSlotInterface
    {
        public Vector2 AngleCoverage => m_AngleCoverage;
        public enum SelectionGraphicState { Normal, Highlighted }  

        [BHeader("Item Wheel Slot", true)]

        [SerializeField]
        [MinMax(0, 360)]
        private Vector2 m_AngleCoverage = Vector2.zero;

        [Space]

        [SerializeField]
        private Image m_SelectionGraphic = null;

        [SerializeField]
        private Color m_SelectionGraphicColor = Color.gray;

        [SerializeField]
        private Color m_SelectionGraphicSelectedColor = Color.white;

        [SerializeField]
        private Color m_SelectionGraphicHighlightedColor = Color.gray;

        [Space]

        [SerializeField]
        private SoundPlayer m_HighlightAudio;


        public override void Select()
        {
            base.Select();

            if(UIManager.ItemWheel.Active)
                m_HighlightAudio.Play2D(ItemSelection.Method.RandomExcludeLast);
        }

        public void SetSlotHighlights(SelectionGraphicState state)
        {
            if (state == SelectionGraphicState.Normal)
            {
                if (m_Selected)
                    m_SelectionGraphic.color = m_SelectionGraphicSelectedColor;
                else
                    m_SelectionGraphic.color = m_SelectionGraphicColor;
            }
            else if (state == SelectionGraphicState.Highlighted)
            {
                m_SelectionGraphic.color = m_SelectionGraphicHighlightedColor;
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (m_AngleCoverage.x < 0 || m_AngleCoverage.y < 0)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.blue;

            Gizmos.DrawRay(transform.parent.position, Quaternion.Euler(0,0, m_AngleCoverage.x) * Vector3.up * 150);
            Gizmos.DrawRay(transform.parent.position, Quaternion.Euler(0,0, m_AngleCoverage.y) * Vector3.up * 150);
        }
        #endif
    }
}