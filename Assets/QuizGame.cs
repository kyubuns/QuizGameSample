using AnimeTask;
using AnKuchen.KuchenList;
using AnKuchen.Map;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizGame : MonoBehaviour
{
    [SerializeField] private UICache titleUiCache = default;
    [SerializeField] private UICache quizUiCache = default;
    [SerializeField] private UICache resultUiCache = default;

    void Start()
    {
        MainGameFlow().Forget();
    }

    private async UniTask MainGameFlow()
    {
        var titleUi = new TitleUiElements(titleUiCache);
        var quizUi = new QuizUiElements(quizUiCache);
        var resultUi = new ResultUiElements(resultUiCache);

        while (true)
        {
            titleUi.Root.SetActive(true);
            quizUi.Root.SetActive(false);
            resultUi.Root.SetActive(false);

            // Title

            var origin = titleUi.TitleText.rectTransform.anchoredPosition;
            await Easing.Create<OutBounce>(origin + new Vector2(0f, 600f), origin, 0.5f).ToAnchoredPosition(titleUi.TitleText);

            await titleUi.StartButton.OnClickAsync();

            titleUi.Root.SetActive(false);
            quizUi.Root.SetActive(true);

            // Quiz

            quizUi.QuizText.text = "6 * 4 = ";
            var buttonClick = new UniTaskCompletionSource<int>();
            using (var editor = quizUi.SelectList.Edit())
            {
                for (var i = 0; i < 100; i++)
                {
                    var i1 = i;
                    editor.Contents.Add(new UIFactory<SelectButtonUiElements>(x =>
                    {
                        x.Text.text = $"{i1}";
                        x.Button.onClick.AddListener(() => buttonClick.TrySetResult(i1));
                    }));
                }
            }

            var index = await buttonClick.Task;

            quizUi.Root.SetActive(false);
            resultUi.Root.SetActive(true);

            // Result

            resultUi.Circle.SetActive(index == 24);
            resultUi.Cross.SetActive(index != 24);

            var animator = Easing.Create<OutBounce>(Vector3.one * 5f, Vector3.one, 0.5f);
            await UniTask.WhenAll(animator.ToLocalScale(resultUi.Circle), animator.ToLocalScale(resultUi.Cross));

            await resultUi.ReturnButton.OnClickAsync();
        }
    }
}

public class TitleUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public TextMeshProUGUI TitleText { get; private set; }
    public Button StartButton { get; private set; }
    public TextMeshProUGUI Text { get; private set; }

    public TitleUiElements() { }
    public TitleUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        TitleText = mapper.Get<TextMeshProUGUI>("TitleText");
        StartButton = mapper.Get<Button>("StartButton");
        Text = mapper.Get<TextMeshProUGUI>("Text");
    }
}

public class QuizUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public TextMeshProUGUI QuizText { get; private set; }
    public Button SelectButton { get; private set; }
    public TextMeshProUGUI Text { get; private set; }
    public VerticalList<SelectButtonUiElements> SelectList { get; private set; }

    public QuizUiElements() { }
    public QuizUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        QuizText = mapper.Get<TextMeshProUGUI>("QuizText");
        SelectButton = mapper.Get<Button>("SelectButton");
        Text = mapper.Get<TextMeshProUGUI>("Text");
        SelectList = new VerticalList<SelectButtonUiElements>(
            mapper.Get<ScrollRect>("Scroll Group 1"),
            mapper.GetChild<SelectButtonUiElements>("SelectButton")
        );
    }
}

public class SelectButtonUiElements : IMappedObject, IReusableMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Button Button { get; private set; }
    public TextMeshProUGUI Text { get; private set; }

    public SelectButtonUiElements() { }
    public SelectButtonUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Button = mapper.Get<Button>();
        Text = mapper.Get<TextMeshProUGUI>("Text");
    }

    public void Activate()
    {
    }

    public void Deactivate()
    {
        Button.onClick.RemoveAllListeners();
    }
}


public class ResultUiElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Button ReturnButton { get; private set; }
    public TextMeshProUGUI Text { get; private set; }
    public GameObject Circle { get; private set; }
    public GameObject Cross { get; private set; }

    public ResultUiElements() { }
    public ResultUiElements(IMapper mapper) { Initialize(mapper); }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        ReturnButton = mapper.Get<Button>("ReturnButton");
        Text = mapper.Get<TextMeshProUGUI>("Text");
        Circle = mapper.Get("Circle");
        Cross = mapper.Get("Cross");
    }
}
