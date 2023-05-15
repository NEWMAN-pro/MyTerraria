using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基于Bowyer-Watson算法 
/// </summary>
public class Voronoi : MonoBehaviour
{


    public int featurePointNum = 50;

    private List<Vector2> featurePoints;//存储特征点
    private List<Edge> trianglesEdgeList;//delaunay三角网格边
    private List<Edge> voronoiEdges;//诺伊图网格边
    List<Triangle> allTriangles;//delaunay三角形

    //初始超级三角形的顶点
    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 pointC;

    void Start()
    {
        voronoiEdges = new List<Edge>();
        featurePoints = new List<Vector2>();
        trianglesEdgeList = new List<Edge>();
        allTriangles = new List<Triangle>();
        SetPoints();
        CreateDelaunay();
        CreateVoronoi();
    }


    private void CreateDelaunay()
    {
        float minX, maxX, minY, maxY, dx, dy, deltaMax, midX, midY;
        minX = featurePoints[0].x;
        minY = featurePoints[0].y;
        maxX = minX;
        maxY = minY;
        //首先根据离散点的坐标信息确定一个矩形框
        for (int i = 0; i < featurePoints.Count; i++)
        {
            if (featurePoints[i].x < minX) minX = featurePoints[i].x;
            if (featurePoints[i].y < minY) minY = featurePoints[i].y;
            if (featurePoints[i].x > maxX) maxX = featurePoints[i].x;
            if (featurePoints[i].y > maxY) maxY = featurePoints[i].y;
        }
        //根据矩形的信息构建一个超级三角形能够包含所有的离散点
        dx = maxX - minX;
        dy = maxY - minY;
        deltaMax = Mathf.Max(dx, dy);
        midX = (minX + maxX) / 2;
        midY = (minY + maxY) / 2;
        pointA = new Vector2(midX - 20 * deltaMax, midY - 20 * deltaMax);
        pointB = new Vector2(midX, midY + 20 * deltaMax);
        pointC = new Vector2(midX + 20 * deltaMax, midY - 20 * deltaMax);
        Triangle tri = new Triangle(pointA, pointB, pointC);
        //将这个三角形作为初始三角形加入集合
        allTriangles.Add(tri);
        SetDelaunayTriangle(allTriangles, featurePoints);
        returnEdgesofTriangleList(allTriangles, out trianglesEdgeList);
    }

    private void CreateVoronoi()
    {
        voronoiEdges = SetVoronoi(allTriangles);
    }
    private void OnRenderObject()
    {
        GL.PushMatrix();
        GL.LoadOrtho();

        GL.Begin(GL.LINES);
        //画德劳内三角形网格
        GL.Color(Color.red);
        for (int i = 0; i < trianglesEdgeList.Count; i++)
        {
            GL.Vertex3(trianglesEdgeList[i]._a.x / Screen.width, trianglesEdgeList[i]._a.y / Screen.height, 0);
            GL.Vertex3(trianglesEdgeList[i]._b.x / Screen.width, trianglesEdgeList[i]._b.y / Screen.height, 0);
        }
        //画特征点
        GL.Color(Color.white);
        for (int i = 0; i < featurePoints.Count; i++)
        {
            GL.Vertex3(featurePoints[i].x / Screen.width, featurePoints[i].y / Screen.height, 0);
            GL.Vertex3((featurePoints[i].x + 3) / Screen.width, (featurePoints[i].y + 3) / Screen.height, 0);
        }
        //画诺伊图
        //GL.Color(Color.blue);
        //for (int i = 0; i < voronoiEdges.Count; i++)
        //{
        //    GL.Vertex3(voronoiEdges[i]._a.x / Screen.width, voronoiEdges[i]._a.y / Screen.height, 0);
        //    GL.Vertex3(voronoiEdges[i]._b.x / Screen.width, voronoiEdges[i]._b.y / Screen.height, 0);
        //}
        GL.End();
        GL.PopMatrix();
    }
    /// <summary>
    /// 根据德劳内三角网格得到诺伊图
    /// </summary>
    private List<Edge> SetVoronoi(List<Triangle> allTriangle)
    {
        List<Edge> voronoiEdgeList = new List<Edge>();
        for (int i = 0; i < allTriangle.Count; i++)
        {
            List<Edge> neighborEdgeList = new List<Edge>();//三角形邻接边集合
            for (int j = 0; j < allTriangle.Count; j++)
            {
                if (j != i)
                {
                    Edge neighborEdge = findCommonEdge(allTriangle[i], allTriangle[j]);
                    if (neighborEdge != null)
                    {
                        neighborEdgeList.Add(neighborEdge);
                        //构造voronoi边
                        Edge voronoiEdge = new Edge(allTriangle[i].center, allTriangle[j].center);
                        if (!voronoiEdgeList.Contains(voronoiEdge))
                            voronoiEdgeList.Add(voronoiEdge);
                    }
                }
            }
            //最外面的三角形，voronoi边需要射线
            //if (neighborEdgeList.Count == 2)
            //{
            //    Vector2 midPoint = Vector2.zero;
            //    Edge rayEdge;
            //    if (isPointOnEdge(neighborEdgeList[0], allTriangle[i].m_Point1) && isPointOnEdge(neighborEdgeList[1], allTriangle[i].m_Point1))
            //    {
            //        midPoint = findMidPoint(allTriangle[i].m_Point2, allTriangle[i].m_Point3);
            //        bool IsObtuseAngle = isPointOnEdge(allTriangle[i].longEdge, allTriangle[i].m_Point2) && isPointOnEdge(allTriangle[i].longEdge, allTriangle[i].m_Point3);
            //        rayEdge = produceRayEdge(allTriangle[i].center, midPoint, allTriangle[i].IsCenterOut, IsObtuseAngle);
            //        voronoiEdgeList.Add(rayEdge);
            //    }
            //    if (isPointOnEdge(neighborEdgeList[0], allTriangle[i].m_Point2) && isPointOnEdge(neighborEdgeList[1], allTriangle[i].m_Point2))
            //    {
            //        midPoint = findMidPoint(allTriangle[i].m_Point1, allTriangle[i].m_Point3);
            //        bool IsObtuseAngle = isPointOnEdge(allTriangle[i].longEdge, allTriangle[i].m_Point1) && isPointOnEdge(allTriangle[i].longEdge, allTriangle[i].m_Point3);
            //        rayEdge = produceRayEdge(allTriangle[i].center, midPoint, allTriangle[i].IsCenterOut, IsObtuseAngle);
            //        voronoiEdgeList.Add(rayEdge);
            //    }
            //    if (isPointOnEdge(neighborEdgeList[0], allTriangle[i].m_Point3) && isPointOnEdge(neighborEdgeList[1], allTriangle[i].m_Point3))
            //    {
            //        midPoint = findMidPoint(allTriangle[i].m_Point2, allTriangle[i].m_Point1);
            //        bool IsObtuseAngle = isPointOnEdge(allTriangle[i].longEdge, allTriangle[i].m_Point2) && isPointOnEdge(allTriangle[i].longEdge, allTriangle[i].m_Point1);
            //        rayEdge = produceRayEdge(allTriangle[i].center, midPoint, allTriangle[i].IsCenterOut, IsObtuseAngle);
            //        voronoiEdgeList.Add(rayEdge);
            //    }
            //}
        }
        return voronoiEdgeList;
    }

    /// <summary>
    /// 构建德劳内三角形网
    /// </summary>
    /// <param name="triangles"></param>
    /// <param name="points"></param>
    private void SetDelaunayTriangle(List<Triangle> allTriangle, List<Vector2> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            List<Triangle> tempTriList = new List<Triangle>();
            //拷贝所有的三角形
            for (int j = 0; j < allTriangle.Count; j++)
            {
                tempTriList.Add(allTriangle[j]);
            }
            //受影响的三角形链表
            List<Triangle> influencedTriangle = new List<Triangle>();
            //新形成的三角形链表
            List<Triangle> newTriangle = new List<Triangle>();
            //受影响的公共边
            List<Edge> commonEdges = new List<Edge>();
            for (int j = 0; j < tempTriList.Count; j++)
            {
                double lengthToCenter = EuclidianDistance(tempTriList[j].center, points[i]);//点到当前三角形外接圆心的距离
                if (lengthToCenter <= tempTriList[j].radius)
                {
                    influencedTriangle.Add(tempTriList[j]);//添加到受影响列表中
                    allTriangle.Remove(tempTriList[j]);//移除当前三角形
                }
            }
            //遍历受影响三角形,得到三角形边
            for (int j = 0; j < influencedTriangle.Count; j++)
            {
                commonEdges.Add(new Edge(influencedTriangle[j].m_Point1, influencedTriangle[j].m_Point2));
                commonEdges.Add(new Edge(influencedTriangle[j].m_Point1, influencedTriangle[j].m_Point3));
                commonEdges.Add(new Edge(influencedTriangle[j].m_Point2, influencedTriangle[j].m_Point3));
            }
            //将受影响的三角形中的公共边所在的新三角形删掉
            if (commonEdges.Count > 0)
            {
                remmoveEdges(commonEdges);
            }
            //将优化后的三角形添加到三角形网中
            for (int j = 0; j < commonEdges.Count; j++)
            {
                allTriangle.Add(new Triangle(commonEdges[j]._a, commonEdges[j]._b, points[i]));
            }
        }
    }
    /// <summary>
    /// 计算点到圆心的距离
    /// </summary>
    /// <param name="p"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    private double EuclidianDistance(Vector2 p, Vector2 p2)
    {
        return Mathf.Sqrt(Mathf.Abs((p.x - p2.x)) * Mathf.Abs((p.x - p2.x)) + Mathf.Abs((p.y - p2.y)) * Mathf.Abs((p.y - p2.y)));
    }

    //找出两个三角形的公共边
    public Edge findCommonEdge(Triangle chgTri1, Triangle chgTri2)
    {
        Edge edge;
        List<Vector2> commonPoints = new List<Vector2>();
        if (PointIsEqual(chgTri1.m_Point1, chgTri2.m_Point1) || PointIsEqual(chgTri1.m_Point1, chgTri2.m_Point2) || PointIsEqual(chgTri1.m_Point1, chgTri2.m_Point3))
        {
            commonPoints.Add(chgTri1.m_Point1);
        }
        if (PointIsEqual(chgTri1.m_Point2, chgTri2.m_Point1) || PointIsEqual(chgTri1.m_Point2, chgTri2.m_Point2) || PointIsEqual(chgTri1.m_Point2, chgTri2.m_Point3))
        {
            commonPoints.Add(chgTri1.m_Point2);
        }
        if (PointIsEqual(chgTri1.m_Point3, chgTri2.m_Point1) || PointIsEqual(chgTri1.m_Point3, chgTri2.m_Point2) || PointIsEqual(chgTri1.m_Point3, chgTri2.m_Point3))
        {
            commonPoints.Add(chgTri1.m_Point3);
        }
        if (commonPoints.Count == 2)
        {
            edge = new Edge(commonPoints[0], commonPoints[1]);
            return edge;
        }
        return null;
    }
    //找出线段中点
    public Vector2 findMidPoint(Vector2 a, Vector2 b)
    {
        return new Vector2((a.x + b.x) / 2.0f, (a.y + b.y) / 2.0f);
    }
    //判断两点是否相同
    public bool PointIsEqual(Vector2 a, Vector2 b)
    {
        if (a.x == b.x && a.y == b.y)
            return true;
        return false;
    }
    //根据两点求以第一个点为起点的射线边
    public Edge produceRayEdge(Vector2 start, Vector2 direction, bool IsCenterOut, bool IsObtuseAngle)
    {
        Vector2 end = Vector2.zero;
        Edge longEdge;

        if (!IsCenterOut)
        {
            end = 2000 * (direction - start);
        }
        else
        {
            if (IsObtuseAngle)
                end = 2000 * (start - direction);
            else end = 2000 * (direction - start);
        }
        longEdge = new Edge(start, end);
        return longEdge;
    }
    //判断点是否在边上
    public bool isPointOnEdge(Edge edge, Vector2 Point)
    {
        if (edge == null) return false;
        if (PointIsEqual(Point, edge._a) || PointIsEqual(Point, edge._b))
            return true;
        return false;
    }
    //移除公共边
    public void remmoveEdges(List<Edge> edges)
    {
        List<Edge> tmpEdges = new List<Edge>();
        //拷贝所有三角形
        for (int i = 0; i < edges.Count; i++)
        {
            tmpEdges.Add(edges[i]);
        }

        for (int i = 0; i < tmpEdges.Count; i++)
        {
            for (int j = i + 1; j < tmpEdges.Count; j++)
            {
                if (IsEdgeEqual(tmpEdges[i], tmpEdges[j]))
                {
                    tmpEdges[i].IsBad = true;
                    tmpEdges[j].IsBad = true;
                }
            }
        }
        edges.RemoveAll((Edge edge) => { return edge.IsBad; });
    }

    public bool IsEdgeEqual(Edge edge1, Edge edge2)
    {
        int samePointNum = 0;
        if (PointIsEqual(edge1._a, edge2._a) || PointIsEqual(edge1._a, edge2._b))
            samePointNum++;
        if (PointIsEqual(edge1._b, edge2._a) || PointIsEqual(edge1._b, edge2._b))
            samePointNum++;
        if (samePointNum == 2)
            return true;
        return false;
    }

    //判断点是否在三角形外接圆的内部
    private bool isInCircle(Triangle triangle, Vector2 Point)
    {
        double lengthToCenter;
        lengthToCenter = EuclidianDistance(triangle.center, Point);
        if (lengthToCenter < triangle.radius)
        {
            return true;
        }
        return false;
    }
    //根据三角形链表返回三角形所有的边
    private void returnEdgesofTriangleList(List<Triangle> allTriangle, out List<Edge> edges)
    {
        List<Edge> commonEdges = new List<Edge>();
        List<Triangle> tempTri = new List<Triangle>();
        for (int i = 0; i < allTriangle.Count; i++)
        {
            tempTri.Add(allTriangle[i]);
        }
        //删除与超级三角形相关的三角形
        //for (int i = 0; i < tempTri.Count; i++)
        //{
        //    if (PointIsEqual(tempTri[i].m_Point1, pointA) || PointIsEqual(tempTri[i].m_Point1, pointB) || PointIsEqual(tempTri[i].m_Point1, pointC))
        //        allTriangle.Remove(tempTri[i]);
        //    else if (PointIsEqual(tempTri[i].m_Point2, pointA) || PointIsEqual(tempTri[i].m_Point2, pointB) || PointIsEqual(tempTri[i].m_Point2, pointC))
        //        allTriangle.Remove(tempTri[i]);
        //    else if (PointIsEqual(tempTri[i].m_Point3, pointA) || PointIsEqual(tempTri[i].m_Point3, pointB) || PointIsEqual(tempTri[i].m_Point3, pointC))
        //        allTriangle.Remove(tempTri[i]);
        //}
        for (int i = 0; i < allTriangle.Count; i++)
        {
            commonEdges.Add(new Edge(allTriangle[i].m_Point1, allTriangle[i].m_Point2));

            commonEdges.Add(new Edge(allTriangle[i].m_Point1, allTriangle[i].m_Point3));

            commonEdges.Add(new Edge(allTriangle[i].m_Point2, allTriangle[i].m_Point3));
        }
        edges = commonEdges;
    }
    /// <summary>
    /// 生成特征点
    /// </summary>
    private void SetPoints()
    {
        featurePoints.Clear();
        Vector2 point;
        System.Random seeder = new System.Random();
        int seed = seeder.Next();
        System.Random rand = new System.Random(seed);
        for (int i = 0; i < featurePointNum; i++)
        {
            point.x = (float)(rand.NextDouble() * Screen.width);
            point.y = (float)(rand.NextDouble() * Screen.height);
            featurePoints.Add(point);
        }
        featurePoints.Sort(new SiteSorterXY());
    }

}

public class Edge
{
    public Vector2 _a, _b;
    public Edge(Vector2 a, Vector2 b)
    {
        _a = a;
        _b = b;
    }
    public bool IsBad;
}

public class Triangle
{
    public Vector2 m_Point1, m_Point2, m_Point3;
    public Vector2 center;
    public double radius;
    public List<Triangle> adjoinTriangle;
    //用来构建诺伊图外围三角形的射线边(前提是德劳内三角形网格去除了和超级三角形相关的三角形)
    public bool IsCenterOut;//根据边的大小关系，判断外接圆圆心在三角形的位置
    public Edge longEdge;//记录最长边

    public Triangle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        m_Point1 = point1;
        m_Point2 = point2;
        m_Point3 = point3;
        center = GetCenter(m_Point1, m_Point2, m_Point3);
    }

    private Vector2 GetCenter(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        //(x-a)^2+(y-b)^2=r^2
        Vector2 center = Vector2.zero;
        center.x = ((p2.y - p1.y) * (p3.y * p3.y - p1.y * p1.y + p3.x * p3.x - p1.x * p1.x) - (p3.y - p1.y) * (p2.y * p2.y - p1.y * p1.y + p2.x * p2.x - p1.x * p1.x)) / (2 * (p3.x - p1.x) * (p2.y - p1.y) - 2 * ((p2.x - p1.x) * (p3.y - p1.y)));
        center.y = ((p2.x - p1.x) * (p3.x * p3.x - p1.x * p1.x + p3.y * p3.y - p1.y * p1.y) - (p3.x - p1.x) * (p2.x * p2.x - p1.x * p1.x + p2.y * p2.y - p1.y * p1.y)) / (2 * (p3.y - p1.y) * (p2.x - p1.x) - 2 * ((p2.y - p1.y) * (p3.x - p1.x)));

        radius = Mathf.Sqrt(Mathf.Abs(p1.x - center.x) * Mathf.Abs(p1.x - center.x) + Mathf.Abs(p1.y - center.y) * Mathf.Abs(p1.y - center.y));

        float L1 = Vector2.Distance(p1, p2);
        float L2 = Vector2.Distance(p1, p3);
        float L3 = Vector2.Distance(p3, p2);
        if (L1 > L2 && L1 > L3)
            longEdge = new Edge(p1, p2);
        if (L2 > L1 && L2 > L3)
            longEdge = new Edge(p1, p3);
        if (L3 > L2 && L3 > L1)
            longEdge = new Edge(p2, p3);

        IsCenterOut = L1 * L1 > (L2 * L2 + L3 * L3) || L2 * L2 > (L1 * L1 + L3 * L3) || L3 * L3 > (L2 * L2 + L1 * L1);
        return center;
    }
}

public class SiteSorterXY : IComparer<Vector2>
{
    public int Compare(Vector2 p1, Vector2 p2)
    {
        if (p1.x > p2.x) return 1;
        if (p1.x < p2.x) return -1;
        return 0;
    }
}


