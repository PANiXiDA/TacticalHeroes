using UnityEngine;

public class PlayManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _preloader;

    private bool isSearching = false;

    public void ToggleSearch()
    {
        if (isSearching)
        {
            EndSearch();
        }
        else
        {
            StartSearch();
        }

        isSearching = !isSearching;
    }

    private void StartSearch()
    {
        _preloader.SetActive(true);
    }

    private void EndSearch()
    {
        _preloader.SetActive(false);
    }
}
