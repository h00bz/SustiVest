
using Xunit;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;

using Microsoft.EntityFrameworkCore;
using SustiVest.Data.Repositories;
using SustiVest.Data.Security;
using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Helpers;

namespace SustiVest.Test
{
    public class ServiceTests : IDisposable
    {
        private readonly DatabaseContext _ctx;
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IAssessmentsService _assessmentsService;
        private readonly IOfferService _offerService;

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
            _offerService = new OfferServiceDb(_ctx);

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
            var request2 = _companyService.CreateRequest(request2Values);
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

        private void Assessment_Add2()
        {
            // Arrange
            Add2CompaniesAnd2Reps();
            var financeRequest1 = _companyService.CreateRequest(new FinanceRequest
            {
                CRNo = "CR123",
                Purpose = "Testing1",
                Amount = 1111,
                Tenor = 11,
                FacilityType = "Credit Line",
                Status = "Approved",
                DateOfRequest = new DateOnly(2021, 1, 1),
                Assessment = true
            });
            var financeRequest2 = _companyService.CreateRequest(new FinanceRequest
            {
                CRNo = "CR456",
                Purpose = "Testing2",
                Amount = 22222,
                Tenor = 22,
                FacilityType = "Term Loan",
                Status = "Approved",
                DateOfRequest = new DateOnly(2021, 1, 1),
                Assessment = true
            });
            var assessment1Values = new Assessments
            {
                RequestNo = financeRequest1.RequestNo,
                AnalystNo = financeRequest1.RepId,
                Sales = 100000,
                EBITDA = 50000,
                DSR = 1.5,
                CCC = 30.0,
                RiskRating = 3,
                MarketPosition = "Good",
                RepaymentStatus = "On Time",
                FinancialLeverage = 2.0,
                WorkingCapital = 50000,
                OperatingAssets = 100000,
                CRNo = financeRequest1.CRNo,
                TotalAssets = 200000,
                NetEquity = 100000
            };

            var assessment2Values = new Assessments
            {
                RequestNo = financeRequest2.RequestNo,
                AnalystNo = financeRequest2.RepId,
                Sales = 200000,
                EBITDA = 100000,
                DSR = 2.0,
                CCC = 60.0,
                RiskRating = 4,
                MarketPosition = "Excellent",
                RepaymentStatus = "Late",
                FinancialLeverage = 3.0,
                WorkingCapital = 100000,
                OperatingAssets = 200000,
                CRNo = financeRequest1.CRNo,
                TotalAssets = 400000,
                NetEquity = 200000
            };

            // Act
            var assessment1 = _assessmentsService.AddAssessment(assessment1Values);
            var assessment2 = _assessmentsService.AddAssessment(assessment2Values);
        }


        [Fact]
        public void GetAssessments_ReturnsAllAssessments()
        {
            // Arrange
            Assessment_Add2();
            var expectedCount = _ctx.Assessments.Count();

            // Act
            var assessments = _assessmentsService.GetAssessments();

            // Assert
            Assert.Equal(expectedCount, assessments.Count);
        }

        [Fact]
        public void GetAssessment_ReturnsAssessmentWithMatchingAssessmentAndRequestNo()
        {
            // Arrange
            Assessment_Add2();

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
            Assessment_Add2();
            var assessment = new Assessments
            {
                RequestNo = 1,
                AnalystNo = 1,
                Sales = 100000,
                EBITDA = 50000,
                DSR = 1.5,
                CCC = 30.0,
                RiskRating = 3,
                MarketPosition = "Good",
                RepaymentStatus = "On Time",
                FinancialLeverage = 2.0,
                WorkingCapital = 50000,
                OperatingAssets = 100000,
                CRNo = "CR123",
                TotalAssets = 200000,
                NetEquity = 100000
            };

            // Act
            var result = _assessmentsService.AddAssessment(assessment);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.RequestNo);
            Assert.Equal(1, result.AnalystNo);
            Assert.Equal(100000, result.Sales);
            Assert.Equal(50000, result.EBITDA);
            Assert.Equal(1.5, result.DSR);
            Assert.Equal(30.0, result.CCC);
            Assert.Equal(3, result.RiskRating);
            Assert.Equal("Good", result.MarketPosition);
            Assert.Equal("On Time", result.RepaymentStatus);
            Assert.Equal(2.0, result.FinancialLeverage);
            Assert.Equal(50000, result.WorkingCapital);
            Assert.Equal(100000, result.OperatingAssets);
            Assert.Equal("CR123", result.CRNo);
            Assert.Equal(200000, result.TotalAssets);
            Assert.Equal(100000, result.NetEquity);
        }

        [Fact]
        public void UpdateAssessment_UpdatesAssessmentInDatabase()
        {
            // Arrange
            Assessment_Add2();
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
            var result = _assessmentsService.UpdateAssessment(assessmentNo, 1, 1, sales, ebitda, dsr, ccc, riskRating, marketPosition, repaymentStatus, financialLeverage, workingCapital, operatingAssets, "CR123", 200000, 100000);

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
            Assessment_Add2();
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
            Assessment_Add2();
            var companyName = "Company1";

            // Act
            var assessments = _assessmentsService.GetAssessmentsByCompanyName(companyName);

            // Assert
            Assert.True(assessments.Count > 0);
            Assert.True(assessments.All(a => a.Company.CompanyName == companyName));
        }

        private void Offer_Add2()
        {
            Assessment_Add2();

            var offer1Values = new Offer
            {
                RequestNo = 1,
                CRNo = "CR123",
                Amount = 10000,
                FundedAmount=5000,
                Tenor = 12,
                Payback = "Monthly",
                Linens = "None",
                Undertakings = "None",
                Covenants = "None",
                ROR = 0.05,
                FacilityType = "Term Loan",
                UtilizationMechanism = "Direct Payment",
                AnalystNo = 1,
                AssessmentNo = 1
            };

            var offer2Values = new Offer
            {
                RequestNo = 2,
                CRNo = "CR456",
                Amount = 20000,
                FundedAmount=10000,
                Tenor = 24,
                Payback = "Quarterly",
                Linens = "None",
                Undertakings = "None",
                Covenants = "None",
                ROR = 0.06,
                FacilityType = "Overdraft",
                UtilizationMechanism = "Cheque",
                AnalystNo = 2,
                AssessmentNo = 2
            };
            
            var offer1=_offerService.CreateOffer(offer1Values);
            var offer2=_offerService.CreateOffer(offer2Values);
        }

        [Fact]
        public void GetOffers_ReturnsAllOffers()
        {
            // Arrange
            Offer_Add2();

            // Act
            var result = _offerService.GetOffers();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetOffer_ReturnsOfferById()
        {
            // Arrange
            Offer_Add2();
            var exprectedOfferId=1;
            var expectedAssessmentNo=1;

            // Act
            var result = _offerService.GetOffer(exprectedOfferId);

            // Assert
            Assert.Equal(exprectedOfferId, result.OfferId);  
            Assert.Equal(expectedAssessmentNo, result.AssessmentNo);

        }

        [Fact]
        public void CreateOffer_AddsNewOfferToDatabase()
        {
            // Arrange
            Assessment_Add2();
            var offer = new Offer { RequestNo = 1, CRNo = "CR123", Amount = 10000, Tenor = 12, Payback = "Monthly", Linens = "None", Undertakings = "None", Covenants = "None", ROR = 0.05, FacilityType = "Term Loan", UtilizationMechanism = "Direct Payment", AnalystNo = 1, AssessmentNo = 1 };

            // Act
            var result = _offerService.CreateOffer(offer);

            // Assert
            Assert.Equal(1, _ctx.Offers.Count());
            Assert.Equal(offer.RequestNo, result.RequestNo);
            Assert.Equal(offer.CRNo, result.CRNo);
            Assert.Equal(offer.Amount, result.Amount);
            Assert.Equal(offer.Tenor, result.Tenor);
            Assert.Equal(offer.Payback, result.Payback);
            Assert.Equal(offer.Linens, result.Linens);
            Assert.Equal(offer.Undertakings, result.Undertakings);
            Assert.Equal(offer.Covenants, result.Covenants);
            Assert.Equal(offer.ROR, result.ROR);
            Assert.Equal(offer.FacilityType, result.FacilityType);
            Assert.Equal(offer.UtilizationMechanism, result.UtilizationMechanism);
            Assert.Equal(offer.AnalystNo, result.AnalystNo);
            Assert.Equal(offer.AssessmentNo, result.AssessmentNo);
        }

        [Fact]
        public void UpdateOffer_UpdatesExistingOfferInDatabase()
        {
            // Arrange
            Offer_Add2();
            var offer = new Offer { Amount = 10000, FundedAmount=5000, Tenor = 12, Payback = "Monthly", Linens = "None", Undertakings = "None", Covenants = "None", ROR = 0.05, FacilityType = "Term Loan", UtilizationMechanism = "Direct Payment" };


            // Act
            var result = _offerService.UpdateOffer(1, 1, "CR123", offer.Amount, offer.FundedAmount, offer.Tenor, offer.Payback, offer.Linens, offer.Undertakings, offer.Covenants, offer.ROR, offer.FacilityType, offer.UtilizationMechanism, 1, 1);

            // Assert

            Assert.Equal(offer.Amount, result.Amount);
            Assert.Equal(offer.Tenor, result.Tenor);
            Assert.Equal(offer.Payback, result.Payback);
            Assert.Equal(offer.Linens, result.Linens);
            Assert.Equal(offer.Undertakings, result.Undertakings);
            Assert.Equal(offer.Covenants, result.Covenants);
            Assert.Equal(offer.ROR, result.ROR);
            Assert.Equal(offer.FacilityType, result.FacilityType);
            Assert.Equal(offer.UtilizationMechanism, result.UtilizationMechanism);

        }

        [Fact]
        public void DeleteOffer_RemovesOfferFromDatabase()
        {
            // Arrange
            Offer_Add2();

            // Act
            var result = _offerService.DeleteOffer(1);

            // Assert
            Assert.True(result);
            Assert.Equal(1, _ctx.Offers.Count());
        }

        [Fact]
        public void GetOffers_ReturnsPagedOffers()
        {
            // Arrange
            Offer_Add2();
            var records=_offerService.GetOffers();

            // Act
            var pagedOffers = _offerService.GetOffers(1, 2);

            // Assert
            Assert.Equal(2, pagedOffers.Data.Count);

        }
        public void Dispose()
        {
            _ctx.Database.EnsureDeleted();
            _ctx.Dispose();

        }
    }
}


