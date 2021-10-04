using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public class DrawingManager : MonoBehaviour
{
    static DrawingManager _instance;

    public static DrawingManager Instance { get { return _instance; } }

    public bool canDraw = true;
    public GameObject drawingInk;

    [Header("Object References")]
    [SerializeField] LineRenderer drawLine;
    [SerializeField] Transform drawingSpot;
    [SerializeField] public List<GameObject> unitPrefabs = new List<GameObject>();
    [SerializeField] Transform unitsParent;

    List<Vector3> drawPoints = new List<Vector3>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        drawLine.startWidth = .6f;
        drawLine.endWidth = .3f;

        //sets the default ink
        drawingInk = unitPrefabs[0];
    }

    Vector3 lastPos = new Vector3();
    Vector3 curPos = new Vector3();
    private void Update()
    {
        if (GameManager.Instance.gamePhase == 0 && canDraw)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DeleteUnitOnClick();

                lastPos = GetVoxelPosFromScreenPos(Input.mousePosition);
                transform.position = lastPos + Vector3.up * 2;
            }
            if (Input.GetMouseButton(0))
            {
                curPos = GetVoxelPosFromScreenPos(Input.mousePosition);

                if (Vector3.Distance(curPos, lastPos) > 0.025f)
                {
                    if (Vector3.Distance(curPos, lastPos) > 0.1f)
                        if (GetVoxelPosFromScreenPosIfValid(Input.mousePosition, drawingInk.GetComponent<Unit>()) != Vector3.zero)
                        {
                            if (GameManager.Instance.money >= drawingInk.GetComponent<Unit>().cost)
                            {
                                Instantiate(drawingInk, GetVoxelPosFromScreenPos(Input.mousePosition), Quaternion.identity, unitsParent);
                                GameManager.Instance.money -= drawingInk.GetComponent<Unit>().cost;
                            }
                        }

                    if (GetPosFromScreenPos(Input.mousePosition) != Vector3.zero)
                        drawPoints.Add(GetPosFromScreenPos(Input.mousePosition) - transform.position + new Vector3(1.15f, 2, -.25f) * 2);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                drawLine.enabled = false;
            }

            drawLine.positionCount = drawPoints.Count;
            drawLine.SetPositions(drawPoints.ToArray());
        }
    }

    void DeleteUnitOnClick ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 250))
        {
            if (hit.collider.tag == "Unit")
                if (hit.collider.GetComponent<Unit>().type == 0)
                    hit.collider.GetComponent<Unit>().DestroyUnit();
        }
    }

    Vector3 GetVoxelPosFromScreenPos (Vector3 pos)
    {
        LayerMask groundMask = LayerMask.GetMask("Ground");

        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 500, groundMask))
        {
            return hit.transform.position + new Vector3(0, 1.5f, 0);
        }

        return Vector3.zero;
    }

    Vector3 GetVoxelPosFromScreenPosIfValid(Vector3 pos, Unit unit)
    {
        LayerMask groundMask = LayerMask.GetMask("Ground");

        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 250, groundMask))
        {
            SquareHandler square = hit.transform.GetComponent<SquareHandler>();

            if (!hit.transform.GetComponent<SquareHandler>().isOccupied && square.i <= TerrainGenerator.Instance.heigth / 2)
            {
                Debug.Log("it got here");
                if (unit.human_cav_siege == 1)
                    square.isOccupied = true;
                if (unit.human_cav_siege == 2)
                {
                    square.isOccupied = true;
                    TerrainGenerator.Instance.terrainGrid[square.i + 1, square.j].isOccupied = true;
                    TerrainGenerator.Instance.terrainGrid[square.i - 1, square.j].isOccupied = true;
                }
                if (unit.human_cav_siege == 3)
                {
                    square.isOccupied = true;
                    TerrainGenerator.Instance.terrainGrid[square.i + 1, square.j].isOccupied = true;
                    TerrainGenerator.Instance.terrainGrid[square.i - 1, square.j].isOccupied = true;

                    TerrainGenerator.Instance.terrainGrid[square.i, square.j + 1].isOccupied = true;
                    TerrainGenerator.Instance.terrainGrid[square.i + 1, square.j + 1].isOccupied = true;
                    TerrainGenerator.Instance.terrainGrid[square.i - 1, square.j + 1].isOccupied = true;

                    TerrainGenerator.Instance.terrainGrid[square.i, square.j - 1].isOccupied = true;
                    TerrainGenerator.Instance.terrainGrid[square.i + 1, square.j - 1].isOccupied = true;
                    TerrainGenerator.Instance.terrainGrid[square.i - 1, square.j - 1].isOccupied = true;
                }

                return hit.transform.position + new Vector3(0, 1.5f, 0);
            }
        }
            

        return Vector3.zero;
    }

    Vector3 GetPosFromScreenPos(Vector3 pos)
    {
        LayerMask groundMask = LayerMask.GetMask("Ground");
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 250, groundMask))
            return hit.point + new Vector3(0, 1.5f, 0);

        return Vector3.zero;
    }

    public GameObject GetPrefabFromUnit (Unit unit)
    {
        return unitPrefabs.Find(x => x.GetComponent<Unit>() == unit);
    }

    //UI BUTTONS

    public void SetInk (int unitIndex)
    {
        drawingInk = GetPrefabFromUnit(LevelSetter.Instance.unitsUnlocked[unitIndex]);
    }
}
