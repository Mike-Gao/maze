using System.Collections.Generic;
using UnityEngine;

public class WeaponHUDManager : MonoBehaviour
{
    [Tooltip("UI panel containing the layoutGroup for displaying weapon ammos")]
    public RectTransform ammosPanel;
    [Tooltip("Prefab for displaying weapon ammo")]
    public GameObject ammoCounterPrefab;

    PlayerWeaponsManager m_PlayerWeaponsManager;

    void Start()
    {
        m_PlayerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();
        DebugUtility.HandleErrorIfNullFindObject<PlayerWeaponsManager, WeaponHUDManager>(m_PlayerWeaponsManager, this);

        WeaponController activeWeapon = m_PlayerWeaponsManager.GetActiveWeapon();
    }

}
