
using Xunit;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;

using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Repositories;
using SustiVest.Data.Security;

namespace SustiVest.Test
{
    public class ServiceTests
    {
        private readonly DatabaseContext _ctx;
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;

        public ServiceTests()
        {
            // configure the data context options to use sqlite for testing
            var options = DatabaseContext.OptionsBuilder
                            .UseSqlite("Data Source= C:\\Users\\ahmed\\Documents\\Masters\\test\\database\\TestDb.db")
                            .LogTo(Console.WriteLine)
                            .Options;

            // create service with new context
            _ctx = new DatabaseContext(options);
            _userService = new UserServiceDb(_ctx);
            _companyService = new CompanyServiceDb(_ctx);

            // recreate the database - ensures a clean database for each test
            Initialise();
        }

        private void Initialise()
        {
            _ctx.Database.EnsureDeleted();
            _ctx.Database.EnsureCreated();
        }

        [Fact]
        public void GetUsers_WhenNoneExist_ShouldReturnNone()
        {
            // act
            var users = _userService.GetUsers();

            // assert
            Assert.Equal(0, users.Count);
        }

        [Fact]
        public void AddUser_When5ValidUsersAdded_ShouldCreate5Users()
        {
            // arrange
             _userService.AddUser("admin", "admin@mail.com", "123456", Role.admin);
             _userService.AddUser("borrower", "borrower@mail.com", "123456", Role.borrower);
             _userService.AddUser("investor", "investor@mail.com", "123456", Role.investor);
             _userService.AddUser("analyst", "analyst@mail.com", "123456", Role.analyst);
             _userService.AddUser("guest", "guest@mail.com", "123456", Role.guest);

            // act
            var users = _userService.GetUsers();
            // assert
            Assert.Equal(5, users.Count); 
        }

        [Fact]
        public void GetPage1WithpageSize2_When5UsersExist_ShouldReturn2Pages()
        {
            // act
            _userService.AddUser("admin", "admin@mail.com", "123456", Role.admin );
            _userService.AddUser("borrower", "borrower@mail.com", "123456", Role.borrower );
            _userService.AddUser("investor", "investor@mail.com", "123456", Role.investor );
            _userService.AddUser("analyst", "analyst@mail.com", "123456", Role.analyst );
            _userService.AddUser("guest", "guest@mail.com", "123456", Role.guest);

            // return first page with 2 users per page
            var pagedUsers = _userService.GetUsers(1,2);

            // assert
            Assert.Equal(3, pagedUsers.TotalPages);
        }

        [Fact]
        public void GetPage1WithPageSize2_When5UsersExist_ShouldReturnPageWith2Users()
        {
            // act
            _userService.AddUser("admin", "admin@mail.com", "123456", Role.admin );
            _userService.AddUser("borrower", "borrower@mail.com", "123456", Role.borrower );
            _userService.AddUser("investor", "investor@mail.com", "123456", Role.investor );
            _userService.AddUser("analyst", "analyst@mail.com", "123456", Role.analyst );
            _userService.AddUser("guest", "guest@mail.com", "123456", Role.guest);

            var pagedUsers = _userService.GetUsers(1,2);

            // assert
            Assert.Equal(2, pagedUsers.Data.Count);
        }

        [Fact]
        public void GetPage1_When0UsersExist_ShouldReturn0Pages()
        {
            // act
            var pagedUsers = _userService.GetUsers(1,2);

            // assert
            Assert.Equal(0, pagedUsers.TotalPages);
            Assert.Equal(0, pagedUsers.TotalRows);
            Assert.Empty(pagedUsers.Data);
        }

        [Fact]
        public void UpdateUser_WhenUserExists_ShouldWork()
        {
            // arrange
            var user = _userService.AddUser("admin", "administrator@mail.com", "123456", Role.admin );

            // act
            user.Name = "administrator";
            user.Email = "administrator@mail.com";            
            var updatedUser = _userService.UpdateUser(user);

            // assert
            Assert.Equal("administrator", updatedUser.Name);
            Assert.Equal("administrator@mail.com", updatedUser.Email);
        }

        [Fact]
        public void Login_WithValidCredentials_ShouldWork()
        {
            // arrange
            _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin );

            // act            
            var user = _userService.Authenticate("admin@mail.com","admin");

            // assert
            Assert.NotNull(user);

        }

        [Fact]
        public void Login_WithInvalidCredentials_ShouldNotWork()
        {
            // arrange
            _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin );

            // act      
            var user = _userService.Authenticate("admin@mail.com","xxx");

            // assert
            Assert.Null(user);

        }

        [Fact]
        public void ForgotPasswordRequest_ForValidUser_ShouldGenerateToken()
        {
            // arrange
            _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin );

            // act      
            var token = _userService.ForgotPassword("admin@mail.com");

            // assert
            Assert.NotNull(token);

        }

        [Fact]
        public void ForgotPasswordRequest_ForInValidUser_ShouldReturnNull()
        {
            // arrange

            // act      
            var token = _userService.ForgotPassword("admin@mail.com");

            // assert
            Assert.Null(token);

        }

        [Fact]
        public void ResetPasswordRequest_WithValidUserAndToken_ShouldReturnUser()
        {
            // arrange
            _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin );
            var token = _userService.ForgotPassword("admin@mail.com");

            // act      
            var user = _userService.ResetPassword("admin@mail.com", token, "password");

            // assert
            Assert.NotNull(user);
            Assert.True(Hasher.ValidateHash(user.Password, "password"));          
        }

        // [Fact]
        // public void ResetPasswordRequest_WithValidUserAndExpiredToken_ShouldReturnNull()
        // {
        //     // arrange
        //     _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin );
        //     var expiredToken = _userService.ForgotPassword("admin@mail.com");
        //     _userService.ForgotPassword("admin@mail.com");

        //     // act      
        //     var user = _userService.ResetPassword("admin@mail.com", expiredToken, "password");

        //     // assert
        //     Assert.Null(user);  
        // }

        [Fact]
        public void ResetPasswordRequest_WithInValidUserAndValidToken_ShouldReturnNull()
        {
            // arrange
            _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin );          
            var token = _userService.ForgotPassword("admin@mail.com");

            // act      
            var user = _userService.ResetPassword("unknown@mail.com", token, "password");

            // assert
            Assert.Null(user);  
        }

        // [Fact]
        // public void ResetPasswordRequests_WhenAllCompleted_ShouldExpireAllTokens()
        // {
        //     // arrange
        //     _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin );       
        //     _userService.AddUser("guest", "guest@mail.com", "guest", Role.guest );          

        //     // create token and reset password - token then invalidated
        //     var token1 = _userService.ForgotPassword("admin@mail.com");
        //     _userService.ResetPassword("admin@mail.com", token1, "password");

        //     // create token and reset password - token then invalidated
        //     var token2 = _userService.ForgotPassword("guest@mail.com");
        //     _userService.ResetPassword("guest@mail.com", token2, "password");

        //     // act  
        //     // retrieve valid tokens 
        //     var tokens = _userService.GetValidPasswordResetTokens();   

        //     // assert
        //     Assert.Empty(tokens);
        // }
        [Fact]
        public void GetCompanies_WhenNoneExist_ShouldReturnEmptyList()
        {
            // Act
            var companies = _companyService.GetCompanies();

            // Assert
            Assert.Empty(companies);
        }

        [Fact]
        public void GetCompanies_WhenCompaniesExist_ShouldReturnListOfCompanies()
        {
            // Arrange
            Initialise();

            var analyst=_userService.AddUser("analystCompanytestor", "analysttestor@mail.com", "123456", Role.analyst);

            _companyService.AddCompany("123cr", "T123", "Company A", "Industry A", new DateOnly(2022, 1, 1), "Activity A", "SME", "Structure A", analyst.Id);
            _companyService.AddCompany("456cr", "T456", "Company B", "Industry B", new DateOnly(2022, 2, 1), "Activity B", "Startup", "Structure B", analyst.Id);


            // Act
            var companies = _companyService.GetCompanies();

            // Assert
            Assert.Equal(2, companies.Count);
            Assert.Contains(companies, c => c.CRNo == "123cr" && c.CompanyName == "Company A");
            Assert.Contains(companies, c => c.CRNo == "456cr" && c.CompanyName == "Company B");
        }

    }
}
