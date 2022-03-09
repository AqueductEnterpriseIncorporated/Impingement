using Impingement.enums;

namespace Impingement.Control
{
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController callingController);

        enumCursorType GetCursorType();
    }
}