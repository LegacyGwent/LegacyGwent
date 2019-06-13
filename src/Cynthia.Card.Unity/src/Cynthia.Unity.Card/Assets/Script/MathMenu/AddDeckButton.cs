using Autofac;
using Cynthia.Card.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDeckButton : MonoBehaviour
{
    private MainCodeService _codeService;

    private void Start()
    {
        _codeService = DependencyResolver.Container.Resolve<MainCodeService>();
    }
    public void OnClick()
    {
        _codeService.AddDeckClick();
    }
}