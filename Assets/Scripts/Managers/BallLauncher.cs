using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    static BallLauncher _instance;
    public static BallLauncher Instance { get { return _instance; } }

    [HideInInspector] public int arrowLevel = 1;

    [SerializeField] Transform launchTarget;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Transform player;
    [SerializeField] Transform bot;
    [SerializeField] float reachTime = 5f;
    [SerializeField] FloatingJoystick joystick;
    LineRenderer line;


    bool onCoolDown = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        if (PlayerPrefs.GetInt("arrow_level") != 0)
            arrowLevel = PlayerPrefs.GetInt("arrow_level");
        else
            arrowLevel = 1;

        line = GetComponent<LineRenderer>();
        line.startWidth = .6f;
        line.endWidth = .3f;
        line.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(BotThrowBall(1));
    }

    private void Update()
    {
        if (GameManager.Instance.gamePhase == 1)
        {
            joystick.gameObject.SetActive(true);

            if (!onCoolDown)
                if (Input.GetMouseButton(0))
                    PropellBallTouch(player, ballPrefab, reachTime);
                else if (Input.GetMouseButtonUp(0))
                    StartCoroutine(OnPropellEnd(player, ballPrefab, reachTime));
        }
    }

    IEnumerator BotThrowBall (float coolDown)
    {
        if (GameManager.Instance.gamePhase == 1)
        {
            Transform ballInstance = Instantiate(ballPrefab, bot.position, Quaternion.identity).transform;
            ballInstance.GetComponent<BallHandler>().throwerIndex = 1;
            BotLaunchBall(ballInstance, 1.5f);
        }

        yield return new WaitForSeconds(coolDown);
        StartCoroutine(BotThrowBall(Random.Range(1, 2.5f)));
    }
    IEnumerator OnPropellEnd(Transform origin, GameObject ball, float reachTime = 1.5f)
    {
        onCoolDown = true;

        if (arrowLevel == 1)
        {
            Transform ballInstance1 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            LaunchBall(ballInstance1, reachTime, launchTarget.position);
        }
        else if (arrowLevel == 2)
        {
            Transform ballInstance1 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance2 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance3 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;

            LaunchBall(ballInstance1, reachTime, launchTarget.position);
            LaunchBall(ballInstance2, reachTime, launchTarget.position + Vector3.left);
            LaunchBall(ballInstance3, reachTime, launchTarget.position + Vector3.right);
        }
        else if (arrowLevel == 3)
        {
            Transform ballInstance1 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance2 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance3 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance4 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance5 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;

            LaunchBall(ballInstance1, reachTime, launchTarget.position);
            LaunchBall(ballInstance2, reachTime, launchTarget.position + Vector3.left);
            LaunchBall(ballInstance3, reachTime, launchTarget.position + Vector3.right);
            LaunchBall(ballInstance4, reachTime, launchTarget.position + Vector3.up);
            LaunchBall(ballInstance5, reachTime, launchTarget.position + Vector3.down);
        }
        else if (arrowLevel == 4)
        {
            Transform ballInstance1 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance2 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance3 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance4 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance5 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance6 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance7 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance8 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;
            Transform ballInstance9 = Instantiate(ball, origin.transform.position, Quaternion.identity).transform;

            LaunchBall(ballInstance1, reachTime, launchTarget.position);
            LaunchBall(ballInstance2, reachTime, launchTarget.position + Vector3.left);
            LaunchBall(ballInstance3, reachTime, launchTarget.position + Vector3.right);
            LaunchBall(ballInstance4, reachTime, launchTarget.position + Vector3.up);
            LaunchBall(ballInstance5, reachTime, launchTarget.position + Vector3.down);
            LaunchBall(ballInstance6, reachTime, launchTarget.position + Vector3.left + Vector3.up);
            LaunchBall(ballInstance7, reachTime, launchTarget.position + Vector3.right + Vector3.down);
            LaunchBall(ballInstance8, reachTime, launchTarget.position + Vector3.left + Vector3.up);
            LaunchBall(ballInstance9, reachTime, launchTarget.position + Vector3.right + Vector3.down);
        }

        line.enabled = false;
        launchTarget.position = new Vector3(player.position.x, 0, player.position.z + 1);

        yield return new WaitForSeconds(1);
        onCoolDown = false;
    }

    void PropellBallTouch(Transform origin, GameObject ball, float reachTime = 1.5f)
    {
        launchTarget.position = new Vector3(joystick.Direction.x, 0, joystick.Direction.y) * -12f + new Vector3(player.position.x, 0, player.position.z * 2 + 5);
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        //    launchTarget.position = hit.point;

        line.enabled = true;
        TrajectoryLine.Render(origin.position, launchTarget.position, reachTime, Color.red, line);
    }

    void LaunchBall(Transform propelObject, float reachTime, Vector3 target)
    {
        Vector3 velocity = TrajectoryMath.CalculateVelocity(propelObject.position, target, reachTime);

        propelObject.GetComponent<Rigidbody>().velocity = velocity;
    }

    void BotLaunchBall(Transform propelObject, float reachTime)
    {
        Vector3 randomTargetPos = new Vector3(Random.Range(-3, 4), 0, Random.Range(0, -20)) + bot.position;
        Vector3 velocity = TrajectoryMath.CalculateVelocity(propelObject.position, randomTargetPos, reachTime);

        propelObject.GetComponent<Rigidbody>().velocity = velocity;
    }
}
