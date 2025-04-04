using UnityEngine.Pool;
using UnityEngine;

public interface IPoolObject
{
    //Текущий номер пула объекта
    int poolNum { get; set; }
    //Функция инициализации объекта
    void Initialize();
    //Функция освобождения обхъекта
    void Release();
}
