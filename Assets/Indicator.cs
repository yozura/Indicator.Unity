using UnityEngine;

public class Indicator : MonoBehaviour
{
    private Rigidbody2D rigid;
    private GameObject instance;
    public GameObject go_indicator;
    public GameObject canvas_indicator;
    public GameObject player;

    private float defaultAngle;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        // 인스턴스 오브젝트 생성 후 canvas에 상속시키기
        instance = Instantiate(go_indicator);
        instance.transform.SetParent(canvas_indicator.transform);
        instance.transform.localScale = new Vector3(1, 1, 1);

        // 기본 각도값 구하기
        Vector2 dir = new Vector2(Screen.width, Screen.height);
        defaultAngle = Vector2.Angle(new Vector2(0, 1), dir);
    }

    private void Update()
    {
        // 움직임 제어
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        rigid.velocity = new Vector3(horizontal, vertical, 0) * 5.0f;

        SetIndicator();
    }

    private bool isOnOffScreen()
    {
        // Enemy의 ViewPort 포지션에 따라
        // 0이랑 같거나 크고 1이랑 같거나 작을 때 끄고
        // 0보다 작거나 1보다 클 때 켜기
        // 유니티 내부 좌표계
        // 1. World Point : 실제 Object의 Position
        // 2. ViewPort Point : 카메라 내부 좌표계 0 ~ 1로 나타냄, 좌측 상단 0.0, 좌측 하단 0.1, 우측 상단 1.0, 우측 하단 0.1
        // Ex) 왼쪽(x <= 0, x가 0보다 작거나 같을 때), 오른쪽(x >= 1, x가 1보다 크거나 같을 때)
        // Ex) 아래쪽(y >= 1, y가 1보다 크거나 같을 때), 위쪽(y <= 0, y가 0보다 작거나 같을 때)
        // 3. Screen Point : 카메라 내에서 오브젝트의 위치를 Pixel(해상도) 단위로 좌표를 매긴 값
        Vector2 vec = Camera.main.WorldToViewportPoint(transform.position);
        if(vec.x >= 0 && vec.x <= 1 && vec.y >= 0 && vec.y <= 1)
        {
            instance.SetActive(false); 
            return false;
        }
        else
        {
            instance.SetActive(true);
            return true;
        }
    }

    private void SetIndicator()
    {
        // Enemy가 카메라 안에 있다면 바로 종료
        if (!isOnOffScreen()) return;

        // UpVector와 Player와 Enemy간의 Vector 사이의 각도를 구한다.
        // 구한 뒤 player의 x좌표가 Enemy의 x좌표보다 클 때 -1을 각도에 곱해주어서 음수로 만들어주고
        // player가 더 작을 때에는 양수를 곱해준다. 이렇게 하면 오른쪽 왼쪽으로 구분할 수 있음.
        float angle = Vector2.Angle(new Vector2(0, 1), (transform.position - player.transform.position));
        int sign = player.transform.position.x > transform.position.x ? -1 : 1;
        angle *= sign;

        // Enemy의 좌표를 ViewPoint로 방향을 구분하기 위해 바꿔주기.
        Vector3 target = Camera.main.WorldToViewportPoint(transform.position);

        // 0.5f를 빼주는 이유는 타겟을 중심에 맞춰주기 위함
        float x = target.x - 0.5f;
        float y = target.y - 0.5f;

        RectTransform indicatorRect = instance.GetComponent<RectTransform>();

        if(-defaultAngle <= angle && angle <= defaultAngle)
        {
            Debug.Log("Up");

            float anchorMinMaxY = 0.96f;
            float anchorMinMaxX = x * (anchorMinMaxY - 0.5f) / y + 0.5f;

            if (anchorMinMaxX >= 0.94f) anchorMinMaxX = 0.94f;
            else if (anchorMinMaxX <= 0.06f) anchorMinMaxX = 0.06f;

            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }
        else if(defaultAngle <= angle && angle <= 180 - defaultAngle)
        {
            Debug.Log("Right");

            float anchorMinMaxX = 0.96f;
            float anchorMinMaxY = (y * (anchorMinMaxX - 0.5f) / x) + 0.5f;

            if (anchorMinMaxY >= 0.96f) anchorMinMaxY = 0.96f;
            else if (anchorMinMaxY <= 0.04f) anchorMinMaxY = 0.04f;

            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }
        else if(-180f + defaultAngle <= angle && angle <= -defaultAngle)
        {
            Debug.Log("Left");

            float anchorMinMaxX = 0.04f;
            float anchorMinMaxY = (y * (anchorMinMaxX - 0.5f) / x) + 0.5f;

            if (anchorMinMaxY >= 0.96f) anchorMinMaxY = 0.96f;
            else if (anchorMinMaxY <= 0.04f) anchorMinMaxY = 0.04f;

            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);

        }
        else if(-180f <= angle && angle <= -180f + defaultAngle || 180 - defaultAngle <= angle && angle <= 180)
        {
            Debug.Log("Down");

            float anchorMinMaxY = 0.04f;
            float anchorMinMaxX = x * (anchorMinMaxY - 0.5f) / y + 0.5f;

            if (anchorMinMaxX >= 0.94f) anchorMinMaxX = 0.94f;
            else if (anchorMinMaxX <= 0.06f) anchorMinMaxX = 0.06f;

            indicatorRect.anchorMin = new Vector2(anchorMinMaxX, anchorMinMaxY);
            indicatorRect.anchorMax = new Vector2(anchorMinMaxX, anchorMinMaxY);
        }

        indicatorRect.anchoredPosition = new Vector3(0, 0, 0);
    }
}
