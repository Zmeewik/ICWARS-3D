    using System;
    using TMPro;
    using UnityEngine;
    using Unity.Collections;
    using System.Collections.Generic;
    using UnityEditor;

    internal class TextPopupControl : MonoBehaviour
    {

        //Ссылка на префаб текста
        [SerializeField] private GameObject TextPrefab;
        //Ссылка на объект отображающий текст
        [SerializeField] private GameObject canvas;

        //Pop up text output
        public void PopUpText(string text, Color color, Vector2 pos)
        {
            GameObject damagePopUp = Instantiate(TextPrefab, canvas.transform);
            damagePopUp.GetComponent<RectTransform>().anchoredPosition = pos;
            TextPopup damageTMPScript = damagePopUp.transform.GetComponent<TextPopup>();
            damageTMPScript.ChangeTextColor(color);
            damageTMPScript.ChangeTime(0.5f, 0.33f);
            damageTMPScript.DamageTextSetUp(text);
        }
    }
