
using TMPro;
using UnityEngine;
using System;
using System.Collections;

internal class TextPopup : MonoBehaviour
{
    //Ссылка на объект текста
    private TextMeshProUGUI textMesh;

    //Время жизни текста без изеенений
    float lifeTime = 0.5f;
    //Время уменьшения альфа канала у текста до уничтожения
    float decreaseTime = 1.5f;
    //Скорость изменения значения
    float changeTime = 3;

    //Изменить цвет текста
    public void ChangeTextColor(Color color)
    {
        if (textMesh == null)
            textMesh = gameObject.GetComponent<TextMeshProUGUI>();
        textMesh.color = color;
    }

    //Изменить время существоваания текста
    public void ChangeTime(float lifeTime, float lifeDecrease)
    {
        this.lifeTime = lifeTime;
        decreaseTime = lifeDecrease;
        changeTime = 1 / (decreaseTime / 0.01f) / Time.deltaTime * 2;
        Debug.Log(changeTime);
    }

    //Установить начальную информацию текста
    public void DamageTextSetUp(string dam)
    {
        textMesh = gameObject.GetComponent<TextMeshProUGUI>();
        textMesh.SetText(dam);
        StartCoroutine(StopMoving());
    }
    //Функция вызываемая каждый кадр
    private void FixedUpdate()
    {
        transform.localScale += new Vector3(1f, 1f, 0) * Time.deltaTime;
    }
    //Корутина жизненного цикла текста
    private IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(lifeTime);
        while (textMesh.color.a > 0)
        {
            var TMPColor = textMesh.color;
            TMPColor.a -= changeTime * Time.deltaTime / 2;
            textMesh.color = TMPColor;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }

}