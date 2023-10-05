
using Xunit;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;

using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Repositories;
using SustiVest.Data.Security;

namespace SustiVest.Test
{
    public class ServiceTests : IDisposable
    {
        private readonly DatabaseContext _ctx;
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;

        private readonly IAssessmentsService _assessmentsService;

        public ServiceTests()
        {
            // configure the data context options to use sqlite for testing
            var options = DatabaseContext.OptionsBuilder
                            .UseSqlite("Data Source= C:\\Users\\ahmed\\Documents\\Masters\\test\\database\\SustiVestTestDB.db")
                            .LogTo(Console.WriteLine)
                            .Options;

            // create service with new context
            _ctx = new DatabaseContext(options);
            _userService = new UserServiceDb(_ctx);
            _companyService = new CompanyServiceDb(_ctx);
            _assessmentsService = new AssessmentServiceDb(_ctx);

            _ctx.Database.EnsureCreated();// recreate the database - ensures a clean database for each test
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
            _userService.AddUser("admin", "admin@mail.com", "123456", Role.admin);
            _userService.AddUser("borrower", "borrower@mail.com", "123456", Role.borrower);
            _userService.AddUser("investor", "investor@mail.com", "123456", Role.investor);
            _userService.AddUser("analyst", "analyst@mail.com", "123456", Role.analyst);
            _userService.AddUser("guest", "guest@mail.com", "123456", Role.guest);

            // return first page with 2 users per page
            var pagedUsers = _userService.GetUsers(1, 2);

            // assert
            Assert.Equal(3, pagedUsers.TotalPages);
        }

        [Fact]
        public void GetPage1WithPageSize2_When5UsersExist_ShouldReturnPageWith2Users()
        {
            // act
            _userService.AddUser("admin", "admin@mail.com", "123456", Role.admin);
            _userService.AddUser("borrower", "borrower@mail.com", "123456", Role.borrower);
            _userService.AddUser("investor", "investor@mail.com", "123456", Role.investor);
            _userService.AddUser("analyst", "analyst@mail.com", "123456", Role.analyst);
            _userService.AddUser("guest", "guest@mail.com", "123456", Role.guest);

            var pagedUsers = _userService.GetUsers(1, 2);

            // assert
            Assert.Equal(2, pagedUsers.Data.Count);
        }

        [Fact]
        public void GetPage1_When0UsersExist_ShouldReturn0Pages()
        {
            // act
            var pagedUsers = _userService.GetUsers(1, 2);

            // assert
            Assert.Equal(0, pagedUsers.TotalPages);
            Assert.Equal(0, pagedUsers.TotalRows);
            Assert.Empty(pagedUsers.Data);
        }

        [Fact]
        public void UpdateUser_WhenUserExists_ShouldWork()
        {
            // arrange
            var user = _userService.AddUser("admin", "administrator@mail.com", "123456", Role.admin);

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
            _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin);

            // act            
            var user = _userService.Authenticate("admin@mail.com", "admin");

            // assert
            Assert.NotNull(user);

        }

        [Fact]
        public void Login_WithInvalidCredentials_ShouldNotWork()
        {
            // arrange
            _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin);

            // act      
            var user = _userService.Authenticate("admin@mail.com", "xxx");

            // assert
            Assert.Null(user);

        }

        [Fact]
        public void ForgotPasswordRequest_ForValidUser_ShouldGenerateToken()
        {
            // arrange
            _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin);

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
            _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin);
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
            _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin);
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
        //     _userService.AddUser("admin", "admin@mail.com", "admin", Role.admin);
        //     _userService.AddUser("guest", "guest@mail.com", "guest", Role.guest);

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

        private void Add2CompaniesAnd2Reps()
        {
            var borrowerUser = _userService.AddUser("borrowerUser", "borroweruser@mail.com", "123456", Role.borrower);
            var borrowerUser2 = _userService.AddUser("borrowerUser2", "borroweruser2@mail.com", "123456", Role.borrower);

            var company1Values = new Company
            {
                CRNo = "CR123",
                TaxID = "T123",
                CompanyName = "Company1",
                Industry = "Technology",
                DateOfEstablishment = new DateOnly(2020, 1, 1),
                Activity = "Software Development",
                Type = "Startup",
                ShareholderStructure = "Private",
                RepId = borrowerUser.Id
            };

            var company2Values = new Company
            {
                CRNo = "CR456",
                TaxID = "T456",
                CompanyName = "Company2",
                Industry = "Finance",
                DateOfEstablishment = new DateOnly(2021, 1, 1),
                Activity = "Investment Banking",
                Type = "SME",
                ShareholderStructure = "Public",
                RepId = borrowerUser2.Id
            };

            // Act
            var company1 = _companyService.AddCompany(company1Values.CRNo, company1Values.TaxID, company1Values.CompanyName, company1Values.Industry, company1Values.DateOfEstablishment, company1Values.Activity, company1Values.Type, company1Values.ShareholderStructure, company1Values.RepId);
            var company2 = _companyService.AddCompany(company2Values.CRNo, company2Values.TaxID, company2Values.CompanyName, company2Values.Industry, company2Values.DateOfEstablishment, company2Values.Activity, company2Values.Type, company2Values.ShareholderStructure, company2Values.RepId);

        }
        [Fact]
        public void AddCompany_AddsNewCompanyToDatabase()
        {
            // Arrange
            var borrowerUser = _userService.AddUser("borrowerUser", "borroweruser@mail.com", "123456", Role.borrower);
            var borrowerUser2 = _userService.AddUser("borrowerUser2", "borroweruser2@mail.com", "123456", Role.borrower);

            var company1Values = new Company
            {
                CRNo = "CR123",
                TaxID = "T123",
                CompanyName = "Company1",
                Industry = "Technology",
                DateOfEstablishment = new DateOnly(2020, 1, 1),
                Activity = "Software Development",
                Type = "Startup",
                ShareholderStructure = "Private",
                RepId = borrowerUser.Id
            };

            var company2Values = new Company
            {
                CRNo = "CR456",
                TaxID = "T456",
                CompanyName = "Company2",
                Industry = "Finance",
                DateOfEstablishment = new DateOnly(2021, 1, 1),
                Activity = "Investment Banking",
                Type = "SME",
                ShareholderStructure = "Public",
                RepId = borrowerUser2.Id
            };

            // Act
            var company1 = _companyService.AddCompany(company1Values.CRNo, company1Values.TaxID, company1Values.CompanyName, company1Values.Industry, company1Values.DateOfEstablishment, company1Values.Activity, company1Values.Type, company1Values.ShareholderStructure, company1Values.RepId);
            var company2 = _companyService.AddCompany(company2Values.CRNo, company2Values.TaxID, company2Values.CompanyName, company2Values.Industry, company2Values.DateOfEstablishment, company2Values.Activity, company2Values.Type, company2Values.ShareholderStructure, company2Values.RepId);

            // Assert
            Assert.NotNull(company1);
            Assert.Equal(company1Values.CRNo, company1.CRNo);
            Assert.Equal(company1Values.TaxID, company1.TaxID);
            Assert.Equal(company1Values.CompanyName, company1.CompanyName);
            Assert.Equal(company1Values.Industry, company1.Industry);
            Assert.Equal(company1Values.DateOfEstablishment, company1.DateOfEstablishment);
            Assert.Equal(company1Values.Activity, company1.Activity);
            Assert.Equal(company1Values.Type, company1.Type);
            Assert.Equal(company1Values.ShareholderStructure, company1.ShareholderStructure);
            Assert.Equal(company1Values.RepId, company1.RepId);

            Assert.NotNull(company2);
            Assert.Equal(company2Values.CRNo, company2.CRNo);
            Assert.Equal(company2Values.TaxID, company2.TaxID);
            Assert.Equal(company2Values.CompanyName, company2.CompanyName);
            Assert.Equal(company2Values.Industry, company2.Industry);
            Assert.Equal(company2Values.DateOfEstablishment, company2.DateOfEstablishment);
            Assert.Equal(company2Values.Activity, company2.Activity);
            Assert.Equal(company2Values.Type, company2.Type);
            Assert.Equal(company2Values.ShareholderStructure, company2.ShareholderStructure);
            Assert.Equal(company2Values.RepId, company2.RepId);
        }

        [Fact]
        public void GetCompanies_WhenCompaniesExist_ShouldReturnListOfCompanies()
        {
            // Arrange

            Add2CompaniesAnd2Reps();

            // Act
            var companies = _companyService.GetCompanies();

            // Assert
            Assert.Equal(2, companies.Count);
            Assert.Contains(companies, c => c.CRNo == "CR123" && c.CompanyName == "Company1");
            Assert.Contains(companies, c => c.CRNo == "CR456" && c.CompanyName == "Company2");
        }

        [Fact]
        public void GetCompanies_ReturnsAllCompanies()
        {
            // Arrange
            Add2CompaniesAnd2Reps();

            var expectedCount = _ctx.Companies.Count();

            // Act
            var companies = _companyService.GetCompanies();

            // Assert
            Assert.Equal(expectedCount, companies.Count);
        }

        [Fact]
        public void GetCompany_ReturnsCompanyWithMatchingCRNo()
        {
            // Arrange
            var crNo = "CR123";
            var expectedName = "Company1";
            Add2CompaniesAnd2Reps();

            // Act
            var company = _companyService.GetCompany(crNo);

            // Assert
            Assert.NotNull(company);
            Assert.Equal(crNo, company.CRNo);
            Assert.Equal(expectedName, company.CompanyName);
        }


        [Fact]
        public void UpdateCompany_UpdatesCompanyInDatabase()
        {
            // Arrange
            Add2CompaniesAnd2Reps();
            var crNo = "CR123";
            var taxID = "T111";
            var companyName = "UpdatedCompany";
            var industry = "Finance";
            var dateOfEstablishment = new DateOnly(2010, 1, 1);
            var activity = "Investment Banking";
            var type = "SME";
            var shareholderStructure = "Public";
            var repId = 2;

            // Act
            var company = _companyService.UpdateCompany(crNo, taxID, companyName, industry, dateOfEstablishment, activity, type, shareholderStructure, repId);

            // Assert
            Assert.NotNull(company);
            Assert.Equal(companyName, company.CompanyName);
            Assert.Equal(industry, company.Industry);
            Assert.Equal(dateOfEstablishment, company.DateOfEstablishment);
            Assert.Equal(activity, company.Activity);
            Assert.Equal(type, company.Type);
            Assert.Equal(shareholderStructure, company.ShareholderStructure);
        }

        [Fact]
        public void DeleteCompany_RemovesCompanyFromDatabase()
        {
            // Arrange
            Add2CompaniesAnd2Reps();
            var crNo = "CR123";

            // Act
            var result = _companyService.DeleteCompany(crNo);

            // Assert
            Assert.True(result);
            Assert.Null(_companyService.GetCompany(crNo));
        }

        private void FinanceRequest_Add2()
        {
            var request1Values = new FinanceRequest
            {
                CRNo = "CR123",
                Purpose = "Testing1",
                Amount = 22222,
                Tenor = 22,
                FacilityType = "Credit Line",
                Status = "Approved",
                DateOfRequest = new DateOnly(2021, 1, 1),
                Assessment = true
            };
            
            var request2Values = new FinanceRequest
            {
                CRNo = "CR456",
                Purpose = "Testing2",
                Amount = 100000,
                Tenor = 12,
                FacilityType = "Term Loan",
                Status = "Pending",
                DateOfRequest = new DateOnly(2021, 1, 1),
                Assessment = false
            };

            // Act
            var request1 = _companyService.CreateRequest(request1Values);
            var request2= _companyService.CreateRequest(request2Values);
        }


        [Fact]
        public void GetFinanceRequests_ReturnsAllFinanceRequests()
        {
            // Arrange
            Add2CompaniesAnd2Reps();
            FinanceRequest_Add2();
            
            var expectedCount = _ctx.FinanceRequests.Count();

            // Act
            var requests = _companyService.GetFinanceRequests();

            // Assert
            Assert.Equal(expectedCount, requests.Count);
        }

        [Fact]
        public void CreateRequest_AddsNewFinanceRequestToDatabase()
        {
            // Arrange
            Add2CompaniesAnd2Reps();
            
            var request = new FinanceRequest
            {
                CRNo = "CR123",
                Purpose = "Testing",
                Amount = 100000,
                Tenor = 12,
                FacilityType = "Term Loan",
                Status = "Pending",
                DateOfRequest = new DateOnly(2021, 1, 1),
                Assessment = false
            };
            _companyService.GetCompany(request.CRNo);

            // Act
            var result = _companyService.CreateRequest(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Testing", result.Purpose);
            Assert.Equal(100000, result.Amount);
            Assert.Equal(12, result.Tenor);
            Assert.Equal("Term Loan", result.FacilityType);
            Assert.Equal("Pending", result.Status);
            Assert.Equal(new DateOnly(2021, 1, 1), result.DateOfRequest);
            Assert.False(result.Assessment);
        }

        [Fact]
        public void UpdateRequest_UpdatesFinanceRequestInDatabase()
        {
            // Arrange
            Add2CompaniesAnd2Reps();
            FinanceRequest_Add2();
            var requestNo = 1;
            var purpose = "Updated Purpose";
            var amount = 200000;
            var tenor = 24;
            var facilityType = "Overdraft";
            var status = "Approved";
            var dateOfRequest = new DateOnly(2021, 2, 1);
            var assessment = true;

            // Act
            var result = _companyService.UpdateRequest(requestNo, purpose, amount, tenor, facilityType, status, dateOfRequest, assessment);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(purpose, result.Purpose);
            Assert.Equal(amount, result.Amount);
            Assert.Equal(tenor, result.Tenor);
            Assert.Equal(facilityType, result.FacilityType);
            Assert.Equal(status, result.Status);
            Assert.Equal(dateOfRequest, result.DateOfRequest);
            Assert.Equal(assessment, result.Assessment);
        }

        [Fact]
        public void ResubmitRequest_UpdatesFinanceRequestInDatabase()
        {
            // Arrange
            Add2CompaniesAnd2Reps();
            FinanceRequest_Add2();
            var requestNo = 1;
            var dateOfRequest = new DateOnly(2021, 3, 1);
            var assessment = false;

            // Act
            var result = _companyService.ResubmitRequest(requestNo, dateOfRequest, assessment);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dateOfRequest, result.DateOfRequest);
            Assert.Equal(assessment, result.Assessment);
        }

        [Fact]
        public void CloseRequest_UpdatesFinanceRequestInDatabase()
        {
            // Arrange
            Add2CompaniesAnd2Reps();
            FinanceRequest_Add2();
            var requestNo = 1;
            var status = "Closed";

            // Act
            var result = _companyService.CloseRequest(requestNo, status);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(status, result.Status);
            Assert.True(result.Assessment);
        }

        [Fact]
        public void DeleteRequest_RemovesFinanceRequestFromDatabase()
        {
            // Arrange
            Add2CompaniesAnd2Reps();
            FinanceRequest_Add2();
            var requestNo = 1;

            // Act
            var result = _companyService.DeleteRequest(requestNo);

            // Assert
            Assert.True(result);
            Assert.Null(_companyService.GetFinanceRequest(requestNo));
        }




        [Fact]
        public void GetAssessments_ReturnsAllAssessments()
        {
            // Arrange
            var expectedCount = _ctx.Assessments.Count();

            // Act
            var assessments = _assessmentsService.GetAssessments();

            // Assert
            Assert.Equal(expectedCount, assessments.Count);
        }

        [Fact]
        public void GetAssessment_ReturnsAssessmentWithMatchingAssessmentNo()
        {
            // Arrange
            var assessmentNo = 1;
            var expectedRequestNo = 1;

            // Act
            var assessment = _assessmentsService.GetAssessment(assessmentNo);

            // Assert
            Assert.NotNull(assessment);
            Assert.Equal(expectedRequestNo, assessment.RequestNo);
        }

        [Fact]
        public void AddAssessment_AddsNewAssessmentToDatabase()
        {
            // Arrange
            var requestNo = 1;
            var analystNo = 1;
            var sales = 100000;
            var ebitda = 50000;
            var dsr = 1.5;
            var ccc = 30.0;
            var riskRating = 3;
            var marketPosition = "Good";
            var repaymentStatus = "On Time";
            var financialLeverage = 2.0;
            var workingCapital = 50000;
            var operatingAssets = 100000;
            var crNo = "1234567890";
            var totalAssets = 200000;
            var netEquity = 100000;
            var assessment = new Assessments
            {
                RequestNo = requestNo,
                AnalystNo = analystNo,
                Sales = sales,
                EBITDA = ebitda,
                DSR = dsr,
                CCC = ccc,
                RiskRating = riskRating,
                MarketPosition = marketPosition,
                RepaymentStatus = repaymentStatus,
                FinancialLeverage = financialLeverage,
                WorkingCapital = workingCapital,
                OperatingAssets = operatingAssets,
                CRNo = crNo,
                TotalAssets = totalAssets,
                NetEquity = netEquity
            };

            // Act
            var result = _assessmentsService.AddAssessment(assessment);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(requestNo, result.RequestNo);
            Assert.Equal(analystNo, result.AnalystNo);
            Assert.Equal(sales, result.Sales);
            Assert.Equal(ebitda, result.EBITDA);
            Assert.Equal(dsr, result.DSR);
            Assert.Equal(ccc, result.CCC);
            Assert.Equal(riskRating, result.RiskRating);
            Assert.Equal(marketPosition, result.MarketPosition);
            Assert.Equal(repaymentStatus, result.RepaymentStatus);
            Assert.Equal(financialLeverage, result.FinancialLeverage);
            Assert.Equal(workingCapital, result.WorkingCapital);
            Assert.Equal(operatingAssets, result.OperatingAssets);
            Assert.Equal(crNo, result.CRNo);
            Assert.Equal(totalAssets, result.TotalAssets);
            Assert.Equal(netEquity, result.NetEquity);
        }

        [Fact]
        public void UpdateAssessment_UpdatesAssessmentInDatabase()
        {
            // Arrange
            var assessmentNo = 1;
            var sales = 200000;
            var ebitda = 100000;
            var dsr = 2.0;
            var ccc = 60.0;
            var riskRating = 4;
            var marketPosition = "Excellent";
            var repaymentStatus = "Late";
            var financialLeverage = 3.0;
            var workingCapital = 100000;
            var operatingAssets = 200000;

            // Act
            var result = _assessmentsService.UpdateAssessment(assessmentNo, 1, 1, sales, ebitda, dsr, ccc, riskRating, marketPosition, repaymentStatus, financialLeverage, workingCapital, operatingAssets, "1234567890", 200000, 100000);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sales, result.Sales);
            Assert.Equal(ebitda, result.EBITDA);
            Assert.Equal(dsr, result.DSR);
            Assert.Equal(ccc, result.CCC);
            Assert.Equal(riskRating, result.RiskRating);
            Assert.Equal(marketPosition, result.MarketPosition);
            Assert.Equal(repaymentStatus, result.RepaymentStatus);
            Assert.Equal(financialLeverage, result.FinancialLeverage);
            Assert.Equal(workingCapital, result.WorkingCapital);
            Assert.Equal(operatingAssets, result.OperatingAssets);
        }

        [Fact]
        public void DeleteAssessment_RemovesAssessmentFromDatabase()
        {
            // Arrange
            var assessmentNo = 1;

            // Act
            var result = _assessmentsService.DeleteAssessment(assessmentNo);

            // Assert
            Assert.True(result);
            Assert.Null(_assessmentsService.GetAssessment(assessmentNo));
        }

        [Fact]
        public void GetAssessmentsByCompanyName_ReturnsAssessmentsWithMatchingCompanyName()
        {
            // Arrange
            var companyName = "Test Company";

            // Act
            var assessments = _assessmentsService.GetAssessmentsByCompanyName(companyName);

            // Assert
            Assert.NotNull(assessments);
            Assert.True(assessments.All(a => a.Company.CompanyName == companyName));
        }
        
        public void Dispose()
        {
            _ctx.Database.EnsureDeleted();
            _ctx.Dispose();

        }
    }
}


