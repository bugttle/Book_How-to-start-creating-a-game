using UnityEngine;

public class Event // イベントの種類
{
    public enum TYPE
    {
        NONE = -1, // なし
        ROCKET = 0, // 宇宙船修理
        NUM, // イベントが何種類あるかを示す（=1）
    }
}
public class EventRoot : MonoBehaviour
{
    public Event.TYPE getEventType(GameObject event_go)
    {
        Event.TYPE type = Event.TYPE.NONE;

        if (event_go != null) // 引数のGameObjectが空っぽでないなら
        {
            if (event_go.tag == "Rocket")
            {
                type = Event.TYPE.ROCKET;
            }
        }
        return (type);
    }

    public bool isEventIgnitable(Item.TYPE carried_item, GameObject event_go)
    {
        bool ret = false;
        Event.TYPE type = Event.TYPE.NONE;

        if (event_go != null)
        {
            type = this.getEventType(event_go); // イベントタイプを取得
        }

        switch (type)
        {
            case Event.TYPE.ROCKET:
                if (carried_item == Item.TYPE.IRON) // 持っているのが鉄鉱石なら
                {
                    ret = true; // 「イベントできるよ！」と返す
                }
                if (carried_item == Item.TYPE.PLANT) // 持っているのが植物なら
                {
                    ret = true; // 「イベントできるよ！」と返す
                }
                break;
        }
        return (ret);
    }

    public string getIgnitableMessage(GameObject event_go)
    {
        string message = "";
        Event.TYPE type = Event.TYPE.NONE;
        if (event_go != null)
        {
            type = this.getEventType(event_go);
        }
        switch (type)
        {
            case Event.TYPE.ROCKET:
                message = "修理する";
                break;
        }
        return (message);
    }
}
