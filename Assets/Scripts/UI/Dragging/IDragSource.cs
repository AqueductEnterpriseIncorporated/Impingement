namespace Impingement.UI.Dragging
{
    /// <summary>
    /// Components that implement this interfaces can act as the source for
    /// dragging a `DragItem`.
    /// </summary>
    /// <typeparam name="T">The type that represents the Item being dragged.</typeparam>
    public interface IDragSource<T> where T : class
    {
        /// <summary>
        /// What Item type currently resides in this source?
        /// </summary>
        T GetItem();

        /// <summary>
        /// What is the quantity of items in this source?
        /// </summary>
        int GetNumber();

        /// <summary>
        /// Remove a given Number of items from the source.
        /// </summary>
        /// <param name="number">
        /// This should never exceed the Number returned by `GetNumber`.
        /// </param>
        void RemoveItems(int number);
    }
}