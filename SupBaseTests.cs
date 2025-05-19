using System.Net.Http.Headers;
using Supbase.Drivers.DriverFactory;
using Supabase.Configs.UniversalMethods;
using Supbase.Posts;
using Microsoft.VisualBasic;
using System.Net;

namespace SupabasecoTests;
[TestFixture]
public class SupBaseTests {
  [SetUp]
  public void Setup() {
      {

        SupbasePostLifecycle.client = new HttpClient();

      }
    }
    [TearDown]

  public void TearDown()

  {

    SupbasePostLifecycle.client.Dispose();

  }

    [Test]
    public async Task TransactionSendAndGet()
    {
        string mail = UniversalMethods.GenerateRandomEmail("test");
        string pass = UniversalMethods.GenerateRandomPassword();
        var payload = new
        {
            email = mail,
            password = pass
        };
        var sendSignup = await UniversalMethods.SendPostRequest($"{SupbasePostLifecycle.BaseUrl}/auth/v1/signup", SupbasePostLifecycle.client, payload);
        Assert.That(UniversalMethods.IsStatusCodeOk(sendSignup));
        var desSingUp = await UniversalMethods.DeserializeSingleOrFirst<SupbasePost>(sendSignup);
        var currentId = desSingUp.User.Id;
        var sendSignIn = await UniversalMethods.SendPostRequest($"{SupbasePostLifecycle.BaseUrl}/auth/v1/token?grant_type=password", SupbasePostLifecycle.client, payload);
        Assert.That(UniversalMethods.IsStatusCodeOk(sendSignIn));
        var desSingIn = await UniversalMethods.DeserializeSingleOrFirst<SupbasePost>(sendSignIn);
        var bearerToken = desSingIn.Access_token;
        var transatcPayload = new
        {
            from_user = currentId,
            to_user = "recipient@test.com",
            amount = 10.5,
            status = "pending"
        };
        var sendTransact = await UniversalMethods.SendPostTransactionRequest($"{SupbasePostLifecycle.BaseUrl}/rest/v1/transactions", SupbasePostLifecycle.client, transatcPayload, bearerToken);
        Assert.That(UniversalMethods.IsStatusCodeCreated(sendTransact), "Transaction should be creatred");
        var desPostTrans = await UniversalMethods.DeserializeSingleOrFirst<SupbasePost>(sendTransact);
        var transaction_id = desPostTrans.Transaction_id;
        var getTransact = await UniversalMethods.SendGetTransactionRequest($"{SupbasePostLifecycle.BaseUrl}/rest/v1/transactions?id=eq.{transaction_id}", SupbasePostLifecycle.client, bearerToken);
        Assert.That(UniversalMethods.IsStatusCodeOk(getTransact));
        var desGetTrans = await UniversalMethods.DeserializeSingleOrFirst<SupbasePost>(getTransact);
        Assert.That(transatcPayload.from_user, Is.EqualTo(desGetTrans.Sender_id));
        Assert.That(transatcPayload.to_user, Is.EqualTo(desGetTrans.To_user));
        Assert.That(transatcPayload.amount, Is.EqualTo(desGetTrans.Amount));
        Assert.That(transatcPayload.status, Is.EqualTo(desGetTrans.Status));
  }
}