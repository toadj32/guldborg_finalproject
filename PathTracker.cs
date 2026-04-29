using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project;

public enum PathType {Path, Hole, Obstacle}

public class PathTracker
{
    public Rectangle Bounds;
    public PathType Type;
    //defining a list of vertices to define the bounds
    public List<Vector2> Vertices;
    private Texture2D Texture; //just be black like the bg for holes

    public PathTracker(Rectangle bounds, PathType type,  Texture2D _texture = null, List<Vector2> vertices = null)
    {
        Bounds = bounds;
        Type = type;
        Vertices = vertices;
        Texture = _texture;
    }

    public Vector2 Center()
    {
        return new Vector2(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
    }
    
    private List<Vector2> DefaultVertices() //so I'm not defining every block
    {
        if (Vertices != null)
            return Vertices;
        List<Vector2> defaultBounds = new List<Vector2>{
            new Vector2(Bounds.Left, Bounds.Top),
            new Vector2(Bounds.Right + 15f, Bounds.Top),
            new Vector2(Bounds.Right+ 15f, Bounds.Bottom+ 15f),
            new Vector2(Bounds.Left, Bounds.Bottom+ 15f)};
        return defaultBounds;
    }

    //closest point on a line segment to another point 
    private Vector2 getClosestPointTo(Vector2 a, Vector2 b, Vector2 point)
    {
        Vector2 ab = b - a;
        Vector2 apoint =  point - a;
        
        //find the amount of overlap
        float projection = Vector2.Dot(apoint, ab);
        //ngl I needed some serious math help with this but from what I understand, 
        //the dot product is ||point - a|| ||b -a|| cos(theta) --> so just dividing by 
        // ||b-a|| is just ||p -a|| * cost (theta) --> this may not be on the segment
        float distanceAlongSegment = projection/ab.LengthSquared();
        distanceAlongSegment = MathHelper.Clamp(distanceAlongSegment, 0, 1);
        return a + distanceAlongSegment * ab;
    }
    
    //normal vector of the obstacle polygon side that is closest to a point, that point being the center of the ball
    public Vector2 getNormal(Vector2 point)
    {
        List<Vector2> vertices = DefaultVertices();
        float bestDist = float.MaxValue;
        Vector2 bestNormal = Vector2.Zero;

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 a = vertices[i];
            Vector2 b = vertices[(i + 1) % vertices.Count]; //for wrapping

            Vector2 closest = getClosestPointTo(a, b, point);
            float distance = Vector2.Distance(point, closest);

            if (distance < bestDist)
            {
                bestDist = distance;
                Vector2 polygonEdge = b - a;
                Vector2 normaltoEdge = new Vector2 (-polygonEdge.Y, polygonEdge.X);
                normaltoEdge.Normalize(); //this might be pointing the otherway depending on what side of hte ball the obstacle is

                Vector2 directionTest = Center() - closest; 
                if (Vector2.Dot(normaltoEdge, directionTest) > 0) //they should be pointing in opposite directions since direction test will be pointing towards osbtacle, and we want normal to edge to point out from obstacle
                    normaltoEdge = -normaltoEdge;
                bestNormal = normaltoEdge;
            }
        }
        return bestNormal;
    }

    public bool inContact(Vector2 ballCenter, float radius)
    {
        List<Vector2> vertices = DefaultVertices();
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 a =  vertices[i];
            Vector2 b = vertices[(i + 1) % vertices.Count];
            Vector2 closest = getClosestPointTo(a, b, ballCenter);
            if (Vector2.Distance(ballCenter, closest) <= radius +2f)
                return true;
        }

        return false;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
        if (Type == PathType.Path)
            return;
        if (Type == PathType.Obstacle && Vertices != null && Vertices.Count >= 3)
            return;
        if (Type == PathType.Hole)
        {
            spriteBatch.Draw(
                Texture,
                new Rectangle(
                    (int)(Bounds.X - cameraPosition.X),
                    (int)(Bounds.Y - cameraPosition.Y),
                    Bounds.Width,
                    Bounds.Height),
                Color.Black);
        }
        
        else if (Type == PathType.Obstacle)
        {
            spriteBatch.Draw(
                Texture,
                new Rectangle(
                    (int)(Bounds.X - cameraPosition.X),
                    (int)(Bounds.Y - cameraPosition.Y),
                    Bounds.Width,
                    Bounds.Height),
                Color.White);
        }
        
    }
    public void DrawNonSquares(
        GraphicsDevice gd, 
        BasicEffect basicEffect,
        Vector2 cameraPosition) //man is it a crime for a girl to just want to draw some triangles
    {
        if (Type != PathType.Obstacle || Vertices == null || Vertices.Count < 3)
            return;

        RasterizerState previousRasterizer = gd.RasterizerState;
        gd.RasterizerState = RasterizerState.CullNone; //see we shouldn't need to see the backside of triangles but nothing showed up when this wasn't disabled

        int triangleCount = Vertices.Count - 2;
        VertexPositionTexture[] vertices = new VertexPositionTexture[triangleCount *3]; //technically not the number of vertices bc overlap
        
        //gotta map the textures to the polygons --> normalize to a 0-1 range
        //finding smallest rectangle that fits the polygon
        float Left = float.MaxValue;
        float Top = float.MaxValue;
        float Right = float.MinValue;
        float Bottom = float.MinValue;
        foreach (var vertex in Vertices)
        {
            if (vertex.X < Left)
                Left = vertex.X;
            if (vertex.Y < Top)
                Top = vertex.Y;
            if (vertex.X > Right)
                Right = vertex.X;
            if (vertex.Y > Bottom)
                Bottom = vertex.Y;
        }

        float polygonWidth = Right - Left;
        float polygonHeight = Bottom - Top;
        if (polygonWidth == 0f)
            polygonWidth = 1f;
        if (polygonHeight == 0f)
            polygonHeight = 1f;
        
        for (int i = 0; i < triangleCount; i++)
        {
            Vector2[] triangle = { Vertices[0], Vertices[i + 1], Vertices[i + 2] };
            for (int j = 0; j < 3; j++)
            {
                Vector2 Position = triangle[j]-cameraPosition;
                vertices[i * 3 + j] =
                    new VertexPositionTexture( //getting the specific vertex within the specified triangle
                        new Vector3(Position.X, Position.Y, 0), //just flat triangle
                        new Vector2(
                            ((triangle[j].X-Left)/polygonWidth), //normalize to the size of the polygon
                            (triangle[j].Y-Top)/polygonHeight));
            }
        }

        basicEffect.TextureEnabled = true;
        basicEffect.VertexColorEnabled = false;
        basicEffect.Texture = Texture;
        basicEffect.PreferPerPixelLighting = false;
        basicEffect.World = Matrix.Identity;
        basicEffect.View = Matrix.Identity;
        basicEffect.Projection = Matrix.CreateOrthographicOffCenter( //representing 3D objects in 2D
            0,
            gd.Viewport.Width,
            gd.Viewport.Height,
            0,
            0,
            1);

        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            gd.DrawUserPrimitives(
                PrimitiveType.TriangleList,
                vertices,
                0,
                triangleCount);
        }

        gd.RasterizerState = previousRasterizer;
    }
    
    
    //basically just check if it's a path or a hole
    public class PathBounds
    {
        private List<PathTracker> _paths = new List<PathTracker>();

        public void AddPath(Rectangle bounds, PathType type, Texture2D texture = null, List<Vector2> vertices = null)
        {
            _paths.Add(new PathTracker(bounds, type, texture, vertices));
        }

        public PathType FindPathType(Vector2 position)
        {
            bool onPath = false; 
            //hierarchy of holes>obstacles>paths
            
            foreach (var path in _paths)
            {
                if (path.Bounds.Contains(position))
                {
                    if (path.Type == PathType.Hole)
                        return PathType.Hole;
                    if (path.Type == PathType.Obstacle)
                        return PathType.Obstacle;
                    if (path.Type == PathType.Path)
                        onPath = true;
                }
            }
            if (onPath)
                return PathType.Path;
            return PathType.Hole; //draw path out and then add obstacles/holes atop it
        }
        
        //basically if it's more than half off the path, fall into the hall
        //ugh back it into a ratio so I can check if the hole is even big enough for the ball to fall through
        public float overlapOnPath(Vector2 ballCenter, float radius)
        {
            const int samplePoints = 16;
            int numPointsOnPath = 0;
            float totalSamplePoints = samplePoints + 1f; //include center

            if (IsOnPathOnly(ballCenter))
                numPointsOnPath++;

            for (int i = 0; i < samplePoints; i++)
            {
                float angle = MathHelper.TwoPi * i / samplePoints;
                Vector2 samplePoint = ballCenter + new Vector2(
                    (float)Math.Cos(angle) * radius,
                    (float)Math.Sin(angle) * radius);
                if (IsOnPathOnly(samplePoint))
                    numPointsOnPath++;
            }

            return numPointsOnPath / totalSamplePoints;
        }
        
        public bool IsOnPathOnly(Vector2 position)
        {
            bool onPath = false;
            foreach (var path in _paths)
            {
                if (!path.Bounds.Contains(position)) 
                    continue;
                if (path.Type == PathType.Hole)     
                    return false;
                if (path.Type == PathType.Obstacle) 
                    continue;
                if (path.Type == PathType.Path)     
                    onPath = true;
            }
            return onPath;
        }

        public PathTracker getObstacle(Vector2 ballCenter, float radius)
        {
            PathTracker obstacle = null;
            float bestDist = float.MaxValue;
            
            foreach (var path in _paths)
            {
                if (path.Type != PathType.Obstacle)
                    continue;
                if (!path.inContact(ballCenter, radius))
                    continue;

                float dist = Vector2.DistanceSquared(ballCenter, path.Center());
                if (dist < bestDist)
                {
                    bestDist = dist;
                    obstacle = path;
                }
            }
            return obstacle;
        }

        public void BoundsDraw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            foreach (var path in _paths)
            {
                if (path.Type == PathType.Path)
                    continue;
                path.Draw(spriteBatch, cameraPosition);
            }
        }

        public void DrawNonSquares(GraphicsDevice gd, BasicEffect basicEffect, Vector2 cameraPosition)
        {
            foreach (var path in _paths)
                path.DrawNonSquares(gd, basicEffect, cameraPosition);
        }
    }
}