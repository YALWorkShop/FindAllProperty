using System.Collections;
using System.ComponentModel;

namespace PracticeProject;

public class ExtendUtil
{
    /// <summary>
    /// 列舉物件的所有屬性,遇到非簡單型別的會有問題
    /// </summary>
    /// <param name="obj">列舉的目標物件</param>
    /// <param name="doSomeThingToProperty">要對他的簡單型別做什麼事?</param>
    public void FindSimpleProperty(object obj, Action<PropertyDescriptor, object> doSomeThingToProperty)
    {
        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj))
        {
            doSomeThingToProperty(property, obj);
        }
    }

    /// <summary>
    /// 列舉物件的所有屬性,遇到自訂物件會再遞迴列舉
    /// </summary>
    /// <param name="obj">列舉的目標物件</param>
    /// <param name="doSomeThingToProperty">要對他的簡單型別做什麼事?</param>
    public void FindAllProperty(object? obj, Action<PropertyDescriptor, object> doSomeThingToProperty)
    {
        if (obj == null) return;

        // 取得物件的每一個欄位的屬性資訊
        foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(obj))
        {
            // 判斷是不是class 從下兩樣取交集
            if (propertyDescriptor.PropertyType.IsClass && !propertyDescriptor.PropertyType.IsValueType
                                                        && propertyDescriptor.PropertyType.UnderlyingSystemType?.Name != "String")
            {
                // 判斷是不是List
                if (
                    propertyDescriptor.PropertyType.IsGenericType &&
                    propertyDescriptor.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // List 處理
                    var data = propertyDescriptor.GetValue(obj);
                    if (data != null)
                    {
                        foreach (var subObj in (IEnumerable)data)
                        {
                            // 遞迴
                            FindAllProperty(subObj, (property, obj1) => CheckAndSetPropertyIsDateTime(property, obj1, new DateTime(2099, 1, 1)));
                        }
                    }
                }
                else
                {
                    // Class包Class用的
                    var value = propertyDescriptor.GetValue(obj);
                    // 遞迴
                    FindAllProperty(value, (property, obj1) => CheckAndSetPropertyIsDateTime(property, obj1, new DateTime(2099, 1, 1)));
                }
            }
            else
            {
                doSomeThingToProperty(propertyDescriptor, obj);
            }
        }
    }

    /// <summary>
    /// 將所有DateTime超過指定日期的全部改成指定日期
    /// </summary>
    /// <param name="property"></param>
    /// <param name="obj"></param>
    /// <param name="targetDateTime"></param>
    public void CheckAndSetPropertyIsDateTime(PropertyDescriptor property, object? obj, DateTime targetDateTime)
    {
        // 取得欄位的值從testObject
        var value = property.GetValue(obj);

        // 判斷是不是dataTime型態 是的話把值轉dateTime型態存入time的欄位
        if (value is not DateTime time) return;

        // 判斷日期
        if (time <= targetDateTime) return;

        // 寫入值
        property.SetValue(obj, targetDateTime);
        Console.WriteLine($"Before Value : {time} || After Value : {property.GetValue(obj)}");
    }
}