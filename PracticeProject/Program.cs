// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using PracticeProject;


// 目標: 找到傳入物件內所有 DateTime 屬性的欄位時間不會大於今天, 如果大於今天則改為今天
var extendUtil = new ExtendUtil();
void DoSomeThingToProperty(PropertyDescriptor property, object obj) =>
    extendUtil.CheckAndSetPropertyIsDateTime(property, obj, DateTime.Now);

// 只有簡單型別的物件處理方式 (比較簡單)
object? testObject = new TestObject() { CreateDate = DateTime.Now.AddDays(10) };

// 利用 TypeDescriptor 去取得物件內每一個欄位的資訊 
extendUtil.FindSimpleProperty(testObject, DoSomeThingToProperty);

var testGroupModel = new TestGroupModel()
{
    TestObject = new TestObject() { CreateDate = DateTime.Now.AddMonths(1) },
    TestObjects = new List<TestObject>()
        {
            new TestObject() { CreateDate = DateTime.Now.AddMonths(-1) },
            new TestObject() { CreateDate = DateTime.Now.AddMonths(2) },
            new TestObject() { CreateDate = DateTime.Now.AddMonths(3) },
        }
};

extendUtil.FindAllProperty(testGroupModel, DoSomeThingToProperty);
Console.WriteLine("End");



public class TestObject
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Tag { get; set; }
    public DateTime CreateDate { get; set; }
}

public class TestGroupModel
{
    public string? Name { get; set; }

    public TestObject? TestObject { get; set; }

    public List<TestObject>? TestObjects { get; set; }
}