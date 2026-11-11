using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Enums;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class CreatorContactsViewModel : ObservableObject
{
    private readonly ICreatorContactService _contactService;
    private readonly IMessenger _messenger;
    
    [ObservableProperty]
    private ObservableCollection<CreatorContact> _contacts = new();
    
    [ObservableProperty]
    private ObservableCollection<string> _availableContactTypes = new();
    
    [ObservableProperty]
    private string _newContactType = "";
    
    [ObservableProperty]
    private string _newContactValue = "";
    
    public long CreatorId { get; set; }
    
    public CreatorContactsViewModel(ICreatorContactService contactService, IMessenger messenger)
    {
        _contactService = contactService;
        _messenger = messenger;
        
        AvailableContactTypes = new ObservableCollection<string>
        {
            "WhatsApp", "Email", "TikTok后台", "达人后台", "其他"
        };
    }
    
    public async Task LoadContactsAsync(long creatorId)
    {
        CreatorId = creatorId;
        var contacts = await _contactService.GetByCreatorIdAsync(creatorId);
        Contacts = new ObservableCollection<CreatorContact>(contacts);
    }
    
    [RelayCommand]
    private async Task AddContactAsync()
    {
        if (string.IsNullOrWhiteSpace(NewContactValue)) return;
        
        var contactType = ParseContactType(NewContactType);
        var contact = await _contactService.AddContactAsync(CreatorId, contactType, NewContactValue);
        Contacts.Add(contact);
        
        NewContactValue = "";
        NewContactType = "";
        
        // 发送更新消息
        _messenger.Send(new CreatorUpdatedMessage());
    }
    
    [RelayCommand]
    private async Task DeleteContactAsync(CreatorContact contact)
    {
        await _contactService.DeleteAsync(contact.Id);
        Contacts.Remove(contact);
        
        // 发送更新消息
        _messenger.Send(new CreatorUpdatedMessage());
    }
    
    private ContactType ParseContactType(string type)
    {
        return type switch
        {
            "WhatsApp" => ContactType.WhatsApp,
            "Email" => ContactType.Email,
            "TikTok后台" => ContactType.TiktokBackend,
            "达人后台" => ContactType.InfluencerBackend,
            _ => ContactType.Other
        };
    }
}
