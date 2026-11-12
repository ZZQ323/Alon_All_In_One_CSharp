using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Enums;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class TodayReminderViewModel : ObservableObject, IRecipient<CreatorUpdatedMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly IMessenger _messenger;

    public string Title => "今日提醒";

    [ObservableProperty] private ObservableCollection<Creator> unreachedCreators = new();
    [ObservableProperty] private ObservableCollection<Creator> agreedCreators = new();
    [ObservableProperty] private ObservableCollection<Creator> sampleSentCreators = new();

    [ObservableProperty] private Creator? selectedCreator;

    public TodayReminderViewModel(ICreatorService creatorService, IMessenger messenger)
    {
        _creatorService = creatorService;
        _messenger = messenger;
        _messenger.Register(this);
    }

    public void Receive(CreatorUpdatedMessage message)
    {
        _ = LoadAsync();
    }

    public async Task LoadAsync()
    {
        var all = await _creatorService.GetAllAsync();
        var today = DateOnly.FromDateTime(DateTime.Today);
        var todayItems = all.Where(c => !c.Deleted && c.NextFollowUpDate.HasValue && c.NextFollowUpDate.Value == today).ToList();

        UnreachedCreators = new ObservableCollection<Creator>(
            todayItems.Where(c => c.CooperationStatus == CooperationStatus.Unreached || c.CooperationStatus == CooperationStatus.Reached));
        AgreedCreators = new ObservableCollection<Creator>(
            todayItems.Where(c => c.CooperationStatus == CooperationStatus.CooperationAgreed));
        SampleSentCreators = new ObservableCollection<Creator>(
            todayItems.Where(c => c.CooperationStatus == CooperationStatus.SampleSent));
    }

    [RelayCommand]
    private async Task MarkRemindedAsync(Creator? creator)
    {
        if (creator == null) return;
        await _creatorService.UpdateTodayStatusAsync(creator.Id, TodayStatus.Reminded);
        _messenger.Send(new CreatorUpdatedMessage());
        await LoadAsync();
    }

    [RelayCommand]
    private async Task AdvanceStatusAsync(Creator? creator)
    {
        if (creator == null) return;
        var next = creator.CooperationStatus switch
        {
            CooperationStatus.Unreached => CooperationStatus.Reached,
            CooperationStatus.Reached => CooperationStatus.CooperationAgreed,
            CooperationStatus.CooperationAgreed => CooperationStatus.SampleSent,
            _ => creator.CooperationStatus
        };
        await _creatorService.UpdateStatusAsync(creator.Id, next);
        _messenger.Send(new CreatorUpdatedMessage());
        await LoadAsync();
    }

    [RelayCommand]
    private async Task MarkNoCooperationAsync(Creator? creator)
    {
        if (creator == null) return;
        await _creatorService.MarkDeliveredAsync(creator.Id, true, "暂不合作");
        _messenger.Send(new CreatorUpdatedMessage());
        await LoadAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync() => await LoadAsync();
}
