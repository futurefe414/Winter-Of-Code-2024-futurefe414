# Design Document

## Overview

SAST Image Client is a modern Windows desktop application built with WinUI 3 that provides a comprehensive image gallery management system. The application follows the MVVM (Model-View-ViewModel) architectural pattern and uses the CommunityToolkit.Mvvm for data binding and command handling. The client communicates with a REST API backend to manage users, categories, albums, images, and tags in a hierarchical structure.

The application provides a rich user interface with navigation, authentication, image browsing, uploading, and management capabilities. It supports both regular users and administrators with different permission levels for various operations.

## Architecture

### High-Level Architecture

The application follows a layered architecture with clear separation of concerns:

```
┌─────────────────────────────────────────┐
│              Presentation Layer          │
│  (Views, ViewModels, Controls, Dialogs) │
├─────────────────────────────────────────┤
│              Service Layer               │
│    (API Services, Authentication)       │
├─────────────────────────────────────────┤
│              Data Layer                  │
│     (DTOs, Contracts, Models)           │
├─────────────────────────────────────────┤
│              Infrastructure              │
│    (HTTP Client, Refit, Helpers)        │
└─────────────────────────────────────────┘
```

### MVVM Pattern Implementation

- **Models**: Data Transfer Objects (DTOs) generated from the API specification
- **Views**: XAML pages and user controls for the user interface
- **ViewModels**: Observable objects that handle business logic and data binding
- **Services**: Abstraction layer for API communication and authentication

### Navigation Architecture

The application uses a Frame-based navigation system:
- **ShellPage**: Main container with NavigationView and TitleBar
- **MainFrame**: Content frame that hosts different pages
- **Navigation Parameters**: Data passed between pages during navigation

## Components and Interfaces

### Core Services

#### SastImgAPI Service
- **Purpose**: Central API client for all backend communication
- **Implementation**: Uses Refit library for REST API calls
- **Interfaces**: IAccountApi, IImageApi, IAlbumApi, ICategoryApi, ITagApi, IUserApi
- **Features**: Automatic JWT token handling, JSON serialization, error handling

#### AuthService
- **Purpose**: Manages user authentication state
- **Features**: Login/logout, token storage, authentication events
- **Events**: LoginStateChanged for UI updates

### UI Components

#### ShellPage
- **Purpose**: Main application shell with navigation and title bar
- **Features**: NavigationView, TitleBar with user avatar, search functionality
- **Navigation**: Handles page routing and back button functionality

#### ExpandableUserAvatar
- **Purpose**: User profile display with expandable menu
- **Features**: Avatar display, login/logout actions, profile access

#### Custom Controls
- **IconButton**: Styled button with icon support
- **TransparentIconButton**: Transparent variant for flyouts

### Page Structure

#### HomeView
- **Purpose**: Main landing page showing categories and featured content
- **Features**: Category grid, recent albums, search integration

#### CategoryView (To be implemented)
- **Purpose**: Display albums within a selected category
- **Features**: Album grid with covers, filtering, sorting

#### AlbumView (To be implemented)
- **Purpose**: Display images within a selected album
- **Features**: Image grid with thumbnails, metadata display

#### ImageView (Existing)
- **Purpose**: Full-screen image display with details
- **Features**: High-resolution image display, metadata, navigation

#### SettingsView
- **Purpose**: Application and user settings
- **Features**: Profile management, preferences, account settings

## Data Models

### Core Entities

#### User Models
```csharp
// User profile information
UserProfileDto {
    long Id
    string Username
    string Biography
}

// Authentication requests
LoginRequest {
    string Username (2-160 chars)
    string Password (6-200 chars)
}

RegisterRequest {
    string Username (2-160 chars)
    string Password (6-200 chars)
    int Code (100000-999999)
}
```

#### Category Models
```csharp
// Category information
CategoryDto {
    long Id
    string Name
    string Description
}

// Category management requests
CreateCategoryRequest {
    string Name (2-500 chars)
    string Description (0-1000 chars)
}
```

#### Album Models
```csharp
// Basic album information
AlbumDto {
    long Id
    string Title
    long Author
    long Category
    DateTimeOffset UpdatedAt
}

// Detailed album information
DetailedAlbum {
    long Id
    string Title
    string Description
    long Author
    long Category
    DateTimeOffset UpdatedAt
    DateTimeOffset CreatedAt
    int AccessLevel (0-4)
    int SubscribeCount
}

// Album creation request
CreateAlbumRequest {
    string Title (1-200 chars)
    string Description (3-600 chars)
    long CategoryId
    int AccessLevel (0-4)
}
```

#### Image Models
```csharp
// Basic image information
ImageDto {
    long Id
    long UploaderId
    long AlbumId
    string Title
    ICollection<long> Tags
    DateTimeOffset UploadedAt
    DateTimeOffset? RemovedAt
}

// Detailed image information
DetailedImage {
    long Id
    long AlbumId
    long UploaderId
    string Title
    DateTimeOffset UploadedAt
    ICollection<long> Tags
    int Likes
    RequesterInfo Requester
}

// Image upload request
Body2 {
    string Title (0-200 chars)
    byte[] Image
    ICollection<long> Tags (max 10)
}
```

#### Tag Models
```csharp
// Tag information
TagDto {
    long Id
    string Name
}

// Tag creation request
CreateTagRequest {
    string Name (1-100 chars)
}
```

### Access Levels
- **0 - Private**: Only author and collaborators can see
- **1 - AuthReadOnly**: Authenticated users can see and read
- **2 - AuthReadWrite**: Authenticated users can see, read, and add
- **3 - PublicReadOnly**: Everyone can see, authenticated users can read
- **4 - PublicReadWrite**: Everyone can see, authenticated users can read and add

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

Before defining the correctness properties, I need to analyze the acceptance criteria for testability:

### Property Reflection

After reviewing all properties identified in the prework analysis, I've identified several areas where properties can be consolidated to eliminate redundancy and provide more comprehensive validation:

**Consolidation Opportunities:**
- Properties 2.1-2.5 (browsing flow) can be combined into comprehensive navigation properties
- Properties 3.1-3.5 (upload flow) can be consolidated into upload workflow properties  
- Properties 4.1-4.5 (album management) can be combined into album lifecycle properties
- Properties 5.1-5.5 (image metadata) can be consolidated into image management properties
- Properties 7.1-7.5 (profile management) can be combined into profile lifecycle properties
- Properties 8.1-8.5 (navigation) can be consolidated into navigation behavior properties
- Properties 9.1-9.5 (error handling) can be combined into comprehensive error handling properties

**Redundancy Elimination:**
- Display format properties (2.4, 4.5, 5.5, 6.4) can be combined into a single metadata display property
- Authentication state properties (1.1, 1.4) can be consolidated into authentication lifecycle property
- API communication properties (3.2, 4.2, 7.2) can be combined into API interaction property

### Correctness Properties

Property 1: Authentication lifecycle consistency
*For any* user with valid credentials, authenticating then logging out should return the system to an unauthenticated state with no residual access
**Validates: Requirements 1.1, 1.4**

Property 2: Invalid authentication rejection
*For any* invalid credentials (malformed username, incorrect password, etc.), authentication attempts should be rejected with appropriate error messaging
**Validates: Requirements 1.2**

Property 3: Registration and auto-authentication
*For any* valid registration data, successful registration should automatically authenticate the user and grant system access
**Validates: Requirements 1.3**

Property 4: Hierarchical navigation consistency
*For any* navigation path (category → album → image), the displayed content should match the selected hierarchy level and contain all required metadata
**Validates: Requirements 2.1, 2.2, 2.3, 2.4, 2.5**

Property 5: File upload validation and processing
*For any* file upload attempt, the system should validate file format and size before processing, and handle both success and failure cases appropriately
**Validates: Requirements 3.1, 3.2, 3.4**

Property 6: Upload completion UI consistency
*For any* successful upload, the UI should immediately reflect the new content and show progress for multiple uploads
**Validates: Requirements 3.3, 3.5**

Property 7: Album lifecycle management
*For any* album operations (create, modify, delete), the system should validate inputs, communicate with the API, and update the UI to reflect changes
**Validates: Requirements 4.2, 4.3, 4.4**

Property 8: Image metadata management
*For any* image editing operations (title, tags, likes), the changes should be persisted and immediately reflected in all relevant UI components
**Validates: Requirements 5.1, 5.2, 5.3, 5.4**

Property 9: Like/unlike round trip consistency
*For any* image, liking then immediately unliking should return the like count and user preference to the original state
**Validates: Requirements 5.3, 5.4**

Property 10: Administrator privilege enforcement
*For any* category management operation, access should be granted only to administrators and denied to regular users with appropriate messaging
**Validates: Requirements 6.2, 6.3, 6.5**

Property 11: Profile management consistency
*For any* profile update operation (information, avatar, password), the changes should be validated, persisted via API, and reflected in the UI
**Validates: Requirements 7.2, 7.3, 7.5**

Property 12: Navigation state consistency
*For any* navigation sequence, the back button state should reflect navigation history availability and parameter passing should work correctly
**Validates: Requirements 8.1, 8.2, 8.3, 8.5**

Property 13: Navigation UI state synchronization
*For any* page navigation, the navigation menu should highlight the current page correctly
**Validates: Requirements 8.4**

Property 14: Error handling and recovery
*For any* error condition (network failure, invalid input, service unavailability), the system should provide appropriate error messages and recovery options
**Validates: Requirements 9.1, 9.2, 9.3, 9.5**

Property 15: Authentication expiry handling
*For any* expired authentication session, the system should detect the expiry and prompt for re-authentication
**Validates: Requirements 9.4**

Property 16: Asynchronous loading with feedback
*For any* large image loading operation, the system should load asynchronously and provide loading indicators
**Validates: Requirements 10.2**

Property 17: Image caching consistency
*For any* frequently accessed image, subsequent access should be faster due to local caching
**Validates: Requirements 10.3**

Property 18: API request reliability
*For any* API request, the system should implement appropriate timeouts and retry logic for reliability
**Validates: Requirements 10.4**

Property 19: Data binding reactivity
*For any* data change, the UI should immediately reflect the change through data binding mechanisms
**Validates: Requirements 10.5**

## Error Handling

### Error Categories

#### Network Errors
- **Connection Failures**: Handle network unavailability with retry mechanisms
- **Timeout Errors**: Implement appropriate timeout values and user feedback
- **API Errors**: Parse and display meaningful error messages from the backend

#### Authentication Errors
- **Invalid Credentials**: Clear error messaging for login failures
- **Token Expiry**: Automatic detection and re-authentication prompts
- **Permission Denied**: Appropriate messaging for insufficient privileges

#### Validation Errors
- **Input Validation**: Client-side validation with immediate feedback
- **File Format Errors**: Clear messaging for unsupported file types
- **Size Limit Errors**: Informative messages about file size restrictions

#### Application Errors
- **Unexpected Exceptions**: Graceful handling with user-friendly messages
- **State Corruption**: Recovery mechanisms for invalid application states
- **Resource Errors**: Handling of memory or storage limitations

### Error Handling Strategy

#### User Experience
- **Progressive Disclosure**: Show basic error messages with option for details
- **Recovery Actions**: Provide clear next steps for error resolution
- **Consistent Messaging**: Use standardized error message formats

#### Technical Implementation
- **Centralized Logging**: Log all errors for debugging and monitoring
- **Error Boundaries**: Prevent error propagation from affecting entire application
- **Graceful Degradation**: Maintain core functionality when non-critical features fail

## Testing Strategy

### Dual Testing Approach

The application will use both unit testing and property-based testing to ensure comprehensive coverage:

#### Unit Testing
- **Specific Examples**: Test concrete scenarios and edge cases
- **Integration Points**: Verify component interactions work correctly
- **UI Components**: Test view models and data binding scenarios
- **API Integration**: Mock API responses to test service layer behavior

#### Property-Based Testing

**Framework**: Microsoft.Pex or FsCheck.NUnit will be used for property-based testing in C#
**Configuration**: Each property-based test will run a minimum of 100 iterations to ensure thorough coverage
**Test Tagging**: Each property-based test will include a comment with the format: `**Feature: sast-image-client, Property {number}: {property_text}**`

**Property Test Requirements**:
- Each correctness property must be implemented by a single property-based test
- Tests must generate realistic test data that respects domain constraints
- Properties should test universal behaviors across all valid inputs
- Test generators should create smart constraints for the input space

**Coverage Areas**:
- Authentication workflows and state management
- Navigation and parameter passing between pages
- Image upload and validation processes
- Album and category management operations
- Error handling and recovery scenarios
- UI state synchronization and data binding

#### Test Organization
- **Unit Tests**: Located alongside source files with `.test.cs` suffix
- **Property Tests**: Organized by feature area in dedicated test projects
- **Integration Tests**: End-to-end scenarios testing complete workflows
- **UI Tests**: Automated testing of user interface interactions

#### Test Data Management
- **Mock Services**: Use mock implementations for API services during testing
- **Test Fixtures**: Predefined data sets for consistent test scenarios
- **Random Generators**: Property-based test generators for comprehensive coverage
- **Image Test Data**: Sample images of various formats and sizes for upload testing

### Performance Testing
- **Load Testing**: Verify performance with large numbers of images and albums
- **Memory Testing**: Ensure proper memory management during image operations
- **Responsiveness Testing**: Verify UI remains responsive during long operations

### Security Testing
- **Authentication Testing**: Verify proper authentication and authorization
- **Input Validation Testing**: Test against malicious input attempts
- **Data Protection Testing**: Ensure sensitive data is handled securely