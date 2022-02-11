using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate.Items
{
    [CustomEditor(typeof(StartupInventory))]
    public class StartupInventoryEditor : Editor
    {
        private StartupInventory m_StartupInventory;
        private Inventory m_Inventory;

        private string[] m_ContainerNames;
        private int m_SelectedContainer;

        private SerializedProperty m_Containers;


        public override void OnInspectorGUI()
        {
            if(m_Inventory == null)
            {
                EditorGUILayout.HelpBox("No Inventory component found!", MessageType.Error);
                return;
            }

            EditorGUILayout.Space();

            m_SelectedContainer = EditorGUILayout.Popup("Container", m_SelectedContainer, m_ContainerNames);

            EditorGUICustom.Separator();
            EditorGUILayout.Space();

            serializedObject.Update();

            if(m_Inventory.StartupContainers.Length != m_ContainerNames.Length)
                CheckContainers();
     
            var container = m_Containers.GetArrayElementAtIndex(m_SelectedContainer);

            GUILayout.BeginHorizontal();
            GUILayout.Space(16f);
            GUILayout.BeginVertical();

            DoContainerGUI(container);

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void DoContainerGUI(SerializedProperty container)
        {
            GUILayout.Label(string.Format("Startup Items ({0})", container.FindPropertyRelative("Name").stringValue + " Container"), EditorStyles.boldLabel);

            var startupItems = container.FindPropertyRelative("StartupItems");

            EditorGUILayout.PropertyField(startupItems);

            GUILayout.Space(16f);

            var itemList = startupItems.GetValue<ItemGeneratorList>();

            if (itemList.Count > 0)
            {
                if (GUILayout.Button("Clear All"))
                {
                    itemList.List.Clear();
                }
            }
        }

        private int FindContainerIndex(string contName)
        {
            int i = 0;

            foreach(SerializedProperty cont in m_Containers)
            {
                if(cont.FindPropertyRelative("Name").stringValue == contName)
                    return i;

                i++;
            }

            return -1;
        }

        private void AddNewContainer(string contName)
        {
            int addIndex = m_Containers.arraySize == 0 ? 0 : m_Containers.arraySize - 1;

            m_Containers.InsertArrayElementAtIndex(addIndex);

            m_Containers.GetArrayElementAtIndex(addIndex).FindPropertyRelative("Name").stringValue = contName;
        }

        private void OnEnable()
        {
            m_StartupInventory = target as StartupInventory;
            m_Inventory = m_StartupInventory.GetComponent<Inventory>();

            if(m_Inventory != null)
            {
                m_Containers = serializedObject.FindProperty("m_ItemContainersStartupItems");
                CheckContainers();
            }
        }

        private void CheckContainers()
        {
            serializedObject.Update();

            PullContainerNames();
            CheckContainersExistence();
            CheckContainersOrder();
            CheckContainersName();

            serializedObject.ApplyModifiedProperties();
        }

        private void PullContainerNames()
        {
            m_ContainerNames = new string[m_Inventory.StartupContainers.Length];

            for (int i = 0; i < m_Inventory.StartupContainers.Length; i++)
            {
                m_ContainerNames[i] = m_Inventory.StartupContainers[i].Name;
            }
        }

        private void CheckContainersExistence()
        {
            if(m_ContainerNames.Length != m_Containers.arraySize)
            {
                foreach(var containerName in m_ContainerNames)
                {
                    int idxOfContainer = FindContainerIndex(containerName);

                    if(idxOfContainer == -1)
                        AddNewContainer(containerName);
                }
            }
        }

        private void CheckContainersOrder()
        {
            for(int i = 0;i < m_ContainerNames.Length;i++)
            {
                int containerIdx = FindContainerIndex(m_ContainerNames[i]);

                // If the order is not right
                if(containerIdx != -1 && containerIdx != i)
                    m_Containers.MoveArrayElement(containerIdx, i);
            }
        }

        private void CheckContainersName()
        {
            int i = 0;

            foreach(SerializedProperty container in m_Containers)
            {
                container.FindPropertyRelative("Name").stringValue = m_ContainerNames[Mathf.Clamp(i, 0, m_ContainerNames.Length - 1)];
                i++;
            }
        }
    }
}