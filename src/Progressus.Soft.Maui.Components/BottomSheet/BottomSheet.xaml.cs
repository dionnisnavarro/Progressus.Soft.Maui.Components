
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Progressus.Soft.Maui.Components;

public partial class BottomSheet : BorderItem, INotifyPropertyChanged
{
    public static readonly BindableProperty TitleProperty =
    BindableProperty.Create(
        propertyName: nameof(Title),
        returnType: typeof(string),
        declaringType: typeof(BottomSheet),
        defaultValue: string.Empty);
    public static readonly BindableProperty DismissibleProperty =
    BindableProperty.Create(
        propertyName: nameof(Dismissible),
        returnType: typeof(bool),
        declaringType: typeof(BottomSheet),
        defaultValue: true);

	public static readonly BindableProperty SheetContentProperty =
	BindableProperty.Create(
		propertyName: nameof(SheetContent),
		returnType: typeof(View),
		declaringType: typeof(BottomSheet),
		defaultValue: true);
	public IView SheetContent
	{
		get => (View)GetValue(SheetContentProperty);
		set => SetValue(SheetContentProperty, value);
	}
	public bool Dismissible
    {
        get => (bool)GetValue(DismissibleProperty);
        set => SetValue(DismissibleProperty, value);
    }
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }
    public BottomSheet()
	{
		InitializeComponent();
	}

    private async void OnCloseButtonClicked(object sender, EventArgs e)
    {
        if(sender is not null)
        {
            var parent = Parent;
            //Find out if component is a child of a modal content page (displayed as modal)
            if(parent is not null && parent is ContentPage && (parent as ContentPage)!.Navigation.ModalStack.Any(l => l.Id == parent.Id))
            {
				await this.TranslateTo(0, 300, 300, Easing.SinInOut);
				await (parent as ContentPage)!.Navigation.PopModalAsync(false);
			}
        }
    }

	/// <summary>
	/// Display alert as a modal window
	/// </summary>
	/// <param name="navigation">Navigation Stack</param>
	/// <param name="title">Alert Title</param>
	/// <param name="instance">Existing instance</param>
	/// <param name="verticalOptions">Vertical Options</param>
	/// <param name="horizontalOptions">Horizontal Options</param>
	/// <param name="overlayColor">Alert Background color</param>
	/// <param name="layout">Custom content page to show alert inside</param>
	/// <returns>A task representing the current operation</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static async Task DisplayAsync(INavigation navigation, string title, View content,
		BottomSheet? instance = null,
		LayoutOptions? verticalOptions = null,
		LayoutOptions? horizontalOptions = null,
		Color? overlayColor = null,
		ContentPage? layout = null)
    {
        if(navigation is null) throw new ArgumentNullException(nameof(navigation));

        //Configure layout
		ContentPage container = layout ?? new();
		//Set background color
        container.BackgroundColor = overlayColor ?? Color.Parse("Transparent");
        
        BottomSheet sheet = instance ?? new()
		{
			Title = title,
			//VerticalOptions = verticalOptions ?? default,
			//HorizontalOptions = horizontalOptions ?? default
			SheetContent = content
		};
        container.Content = sheet;
		await navigation.PushModalAsync(container, false);
		await sheet.TranslateTo(0, 0, 300, Easing.SinInOut);
	}
}