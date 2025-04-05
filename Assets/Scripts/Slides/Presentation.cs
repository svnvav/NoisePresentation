using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Plarium.Tools.NoisePresentation
{
    public class Presentation : MonoBehaviour
    {
        [Header("Navigation Buttons")]
        [SerializeField] private Button _contents;
        [SerializeField] private Button _prev;
        [SerializeField] private Button _next;

        [Header("Slides")] 
        [SerializeField] private float _transitionTime = 0.5f;
        
        private ISlide[] _slides;

        private int _currentSlide;
        private int _targetSlide;

        private bool _processing;

        private Coroutine _processRoutine;

        private void Awake()
        {
            var slidesList = new List<ISlide>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    slidesList.Add(child.GetComponent<ISlide>());
                }
            }

            _slides = slidesList.ToArray();
        }

        private IEnumerator Start()
        {
            _targetSlide = 0;
            _currentSlide = _targetSlide;
            UpdateNavButtonsEnabled();
            yield return _slides[_currentSlide].DoEnter(_transitionTime);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightArrow) && _targetSlide != _slides.Length - 1) NextSlide();
            if(Input.GetKeyDown(KeyCode.LeftArrow) && _targetSlide != 0) PrevSlide();
        }

        public void QuitApp()
        {
            Application.Quit();
        }
        
        public void NextSlide()
        {
            if (_targetSlide < _slides.Length - 1)
            {
                _targetSlide++;
                if (!_processing) _processRoutine = StartCoroutine(ProcessSlides());
                UpdateNavButtonsEnabled();
            }
        }

        public void PrevSlide()
        {
            if (_targetSlide > 0)
            {
                _targetSlide--;
                if (!_processing) _processRoutine = StartCoroutine(ProcessSlides());
                UpdateNavButtonsEnabled();
            }
        }

        public void GoToSlide(int slideIndex)
        {
            if(_processing) StopCoroutine(_processRoutine);

            _targetSlide = slideIndex;
            StartCoroutine(GoToSlideRoutine());
        }

        private IEnumerator GoToSlideRoutine()
        {
            var speed = Mathf.Abs(_targetSlide - _currentSlide);
            _transitionTime /= speed;
            _contents.interactable = false;
            _prev.interactable = false;
            _next.interactable = false;

            yield return ProcessSlides();
            
            _contents.interactable = true;
            UpdateNavButtonsEnabled();
            _transitionTime *= speed;
        }

        private IEnumerator ProcessSlides()
        {
            _processing = true;
            while (_targetSlide != _currentSlide)
            {
                if (_targetSlide > _currentSlide)
                {
                    yield return _slides[_currentSlide].DoExit(_transitionTime * 0.5f);
                    _currentSlide++;
                    yield return _slides[_currentSlide].DoEnter(_transitionTime * 0.5f);
                }
                else
                {
                    yield return _slides[_currentSlide].DoBack(_transitionTime * 0.5f);
                    _currentSlide--;
                    yield return _slides[_currentSlide].DoEnterFromBack(_transitionTime * 0.5f);
                }
            }

            _processing = false;
        }
        
        private void UpdateNavButtonsEnabled()
        {
            _prev.interactable = _targetSlide != 0;
            _next.interactable = _targetSlide != _slides.Length - 1;
        }
    }
}