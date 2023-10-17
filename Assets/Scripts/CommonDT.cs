public enum LevelState
{
    InProgress,
    Failed,
    Completed
}
public struct ShotData
{
    public float PositionX;
    public float PositionY;
    public float PositionZ;
    public float DirectionX;
    public float DirectionY;
    public float VelocityX;
    public float VelocityY;

    public ShotData(float positionX, float positionY, float positionZ, float directionX, float directionY, float velocityX, float velocityY)
    {
        PositionX = positionX;
        PositionY = positionY;
        PositionZ = positionZ;
        DirectionX = directionX;
        DirectionY = directionY;
        VelocityX = velocityX;
        VelocityY = velocityY;
    }
}
