using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using HQFPSTemplate.Items;

namespace HQFPSTemplate.UserInterface
{
    public class UI_ItemWheel : UserInterfaceBehaviour
    {
        [BHeader("Main Settings", true)]

        [SerializeField]
        [PlayerItemContainer]
        private string m_HolsterContainerName = string.Empty;

        [SerializeField]
        private KeyCode m_ItemWheelKey = KeyCode.Q;

        [Space]

        [SerializeField]
        private Panel m_Panel = null;

        [Space]

        [SerializeField]
        [Range(0f, 2f)]
        private float m_ToggleCooldown = 0.25f;

        [SerializeField]
        [Range(0.1f, 25f)]
        private float m_Sensitivity = 3f;

        [SerializeField]
        [Range(0.1f, 25f)]
        private float m_Range = 3f;

        [BHeader("Description...")]

        [SerializeField]
        private Text m_DescriptionText;

        [SerializeField]
        private Text m_ItemNameText;

        [SerializeField]
        private GameObject m_TileSeparator;

        [BHeader("Effects...")]

        [SerializeField]
        private bool m_EnableWheelArrow = false;

        [SerializeField]
        [ShowIf("m_EnableWheelArrow", true)]
        private RectTransform m_WheelArrow = null;

        [SerializeField]
        private bool m_SlowDownTime = false;

        [SerializeField]
        [ShowIf("m_SlowDownTime", true)]
        private float m_SlowDownTimeMultiplier = 0.35f;

        [SerializeField]
        [ShowIf("m_SlowDownTime", true)]
        private float m_SlowDownTimeSpeed = 3f;

        [SerializeField]
        private float m_BackgroundAudioVolume = 0.25f;

        private Dictionary<UI_WheelSlot, ItemSlot> m_SlotDictionary = new Dictionary<UI_WheelSlot, ItemSlot>();
        private UI_WheelSlot[] m_WheelSlots;

        private ItemContainer m_HolsterContainer;

        private int m_LastHighlightedSlot;
        private int m_LastSelectedSlot = -1;

        private Vector2 m_CursorPos;
        private Vector2 m_DirectionOfSelection;

        private float m_NextTimeCanToggleWheel;
        private bool m_IsVisible;

        private AudioSource m_AudioSource;


        public override void OnAttachment()
        {
            if (!gameObject.activeSelf)
                return;

            m_HolsterContainer = Player.Inventory.GetContainerWithName(m_HolsterContainerName);
            m_WheelSlots = GetComponentsInChildren<UI_WheelSlot>();

            if (m_HolsterContainer != null)
            {
                for(int i = 0;i < m_HolsterContainer.Count;i++)
                {
                    m_SlotDictionary.Add(m_WheelSlots[i], m_HolsterContainer[i]);
                    m_WheelSlots[i].LinkToSlot(m_HolsterContainer[i]);
                }

                m_HolsterContainer.SelectedSlot.AddChangeListener(HandleSlotSelection);
            }

            UIManager.ItemWheel.SetStartTryer(TryStart_ItemWheelInspection);
            UIManager.ItemWheel.SetStopTryer(TryStop_ItemWheelInspection);
        }

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();

            if (!m_EnableWheelArrow && m_WheelArrow != null)
                m_WheelArrow.gameObject.SetActive(false);
        }

        private bool TryStart_ItemWheelInspection()
        {
            if (!Player.Aim.Active && Time.time > m_NextTimeCanToggleWheel && !Player.Healing.Active)
            {
                m_Panel.TryShow(true);
                m_NextTimeCanToggleWheel = Time.time + m_ToggleCooldown;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (m_AudioSource != null)
                    m_AudioSource.volume = m_BackgroundAudioVolume;

                return true;
            }

            return false;
        }

        private bool TryStop_ItemWheelInspection()
        {
            if (Time.time > m_NextTimeCanToggleWheel)
            {
                m_Panel.TryShow(false);

                m_HolsterContainer.SelectedSlot.Set(m_LastHighlightedSlot);
                HandleSlotSelection(m_LastHighlightedSlot);

                m_NextTimeCanToggleWheel = Time.time + m_ToggleCooldown;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (m_AudioSource != null)
                    m_AudioSource.volume = 0f;

                return true;
            }

            return false;
        }

        private void Update()
        {
            if (Input.GetKey(m_ItemWheelKey))
            {
                if (!UIManager.ItemWheel.Active)
                {
                    if (UIManager.ItemWheel.TryStart())
                        Player.ViewLocked.Set(true);

                    TryShowSlotInfo(m_WheelSlots[m_LastSelectedSlot]);
                }
            }
            else if (UIManager.ItemWheel.Active && UIManager.ItemWheel.TryStop())
                Player.ViewLocked.Set(false);

            if (!m_Panel.IsVisible)
            {
                if (m_IsVisible && m_SlowDownTime)
                {
                    StopAllCoroutines();
                    StartCoroutine(C_LerpTimeScale(1f));
                    m_IsVisible = false;
                }

                return;
            }

            if (!m_IsVisible)
            {
                if (m_LastSelectedSlot != -1)
                {
                    TryShowSlotInfo(m_WheelSlots[m_LastSelectedSlot]);

                    if (!m_WheelSlots[m_LastSelectedSlot].HasItem)
                    {
                        m_WheelSlots[m_LastSelectedSlot].Deselect();
                        m_WheelSlots[m_LastSelectedSlot].SetSlotHighlights(UI_WheelSlot.SelectionGraphicState.Normal);
                        m_LastSelectedSlot = -1;
                    }
                }

                m_IsVisible = true;

                if (m_SlowDownTime)
                    StartCoroutine(C_LerpTimeScale(m_SlowDownTimeMultiplier));
            }

            int highlightedSlot = GetHighlightedSlot();

            if(highlightedSlot != m_LastHighlightedSlot && highlightedSlot != -1)
                HandleSlotHighlighting(highlightedSlot);
        }

        private int GetHighlightedSlot()
        {
            Vector2 directionOfSelection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).normalized * m_Range;

            if(directionOfSelection != Vector2.zero) 
                m_DirectionOfSelection = Vector2.Lerp(m_DirectionOfSelection, directionOfSelection, Time.deltaTime * m_Sensitivity);

            m_CursorPos = m_DirectionOfSelection;
            
            float angle = -Vector2.SignedAngle(Vector2.up, m_CursorPos);

            if (angle < 0)
                angle = 360f - Mathf.Abs(angle);

            angle = 360f - angle;

            // Rotate the wheel arrow to point to the corresponding angle
            if (m_EnableWheelArrow)
                m_WheelArrow.rotation = Quaternion.Euler(0f, 0f, angle);

            for (int i = 0; i < m_WheelSlots.Length; i++)
            {
                if (angle >= m_WheelSlots[i].AngleCoverage.x && angle <= m_WheelSlots[i].AngleCoverage.y)
                    return i;
            }

            return -1;
        }

        private void HandleSlotHighlighting(int highlightedSlot)
        {
            m_WheelSlots[highlightedSlot].SetSlotHighlights(UI_WheelSlot.SelectionGraphicState.Highlighted);
            m_WheelSlots[highlightedSlot].Select();

            if (m_LastSelectedSlot != m_LastHighlightedSlot)
                m_WheelSlots[m_LastHighlightedSlot].Deselect();

            m_WheelSlots[m_LastHighlightedSlot].SetSlotHighlights(UI_WheelSlot.SelectionGraphicState.Normal);

            m_LastHighlightedSlot = highlightedSlot;

            // Show all of the info related to the highlighted item
            TryShowSlotInfo(m_WheelSlots[highlightedSlot]);
        }

        private void HandleSlotSelection(int selectedSlot)
        {
            if (selectedSlot != m_LastSelectedSlot)
            {            
                if (m_LastSelectedSlot != -1)
                {
                    m_WheelSlots[selectedSlot].Select();
                    m_WheelSlots[selectedSlot].SetSlotHighlights(UI_WheelSlot.SelectionGraphicState.Highlighted);

                    m_WheelSlots[m_LastSelectedSlot].Deselect();
                    m_WheelSlots[m_LastSelectedSlot].SetSlotHighlights(UI_WheelSlot.SelectionGraphicState.Normal);

                    m_LastSelectedSlot = selectedSlot;
                }
                else
                {
                    m_WheelSlots[selectedSlot].Select();
                    m_WheelSlots[selectedSlot].SetSlotHighlights(UI_WheelSlot.SelectionGraphicState.Highlighted);
                    

                    m_LastSelectedSlot = selectedSlot;
                }
            }
        }

        private void TryShowSlotInfo(UI_WheelSlot slot)
        {
            if (m_SlotDictionary.TryGetValue(slot, out ItemSlot itemSlot))
            {
                if (itemSlot != null && itemSlot.HasItem)
                {
                    m_ItemNameText.text = itemSlot.Item.Name;

                    if(itemSlot.Item.Info.Description.Length > 0)
                        m_DescriptionText.text = itemSlot.Item.Info.Description;

                    if(m_TileSeparator != null && !m_TileSeparator.activeSelf)
                        m_TileSeparator.SetActive(true);
                }
                else
                {
                    m_ItemNameText.text = "Unarmed";
                    m_DescriptionText.text = "";

                    if (m_TileSeparator != null)
                        m_TileSeparator.SetActive(false);
                }
            }
        }

        private IEnumerator C_LerpTimeScale(float targetTimeScale) 
        {
            while (Time.timeScale != targetTimeScale) 
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, m_SlowDownTimeSpeed);

                yield return null;
            }
        }
    }
}
