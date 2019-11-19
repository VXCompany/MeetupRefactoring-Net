﻿using Moq;
using TripServiceKataCore.Exception;
using TripServiceKataCore.Tests.Builders;
using TripServiceKataCore.Trips;
using TripServiceKataCore.Users;
using Xunit;

namespace TripServiceKataCore.Tests
{
    public class TripServiceTest
    {
        private static readonly User _guest = null;
        private static readonly User _unknownUser = new UserBuilder();
        private static readonly User _loggedInUser = new UserBuilder();

        private readonly TripService _tripService;

        public TripServiceTest()
        {
            var tripDAOmock = new Mock<TripDAO>();
            tripDAOmock.Setup(x => x.FindTripsByUser(It.IsAny<User>())).Returns<User>(x => x.Trips());
            _tripService = new TripService(tripDAOmock.Object);
        }

        [Fact]
        public void Should_Throw_Exception_If_User_Is_Not_Logged_In()
        {
            Assert.Throws<UserNotLoggedInException>(() => _tripService.GetTripsByUser(_unknownUser, _guest));
        }

        [Fact]
        public void Should_Not_Return_Trips_When_User_Is_Not_A_Friend()
        {
            var trips = _tripService.GetTripsByUser(_unknownUser, _loggedInUser);
            Assert.Empty(trips);
        }

        [Fact]
        public void Should_Return_Trips_When_User_Is_A_Friend()
        {
            var friend = new UserBuilder().WithFriend(_loggedInUser).WithTrip(new Trip());
            var trips = _tripService.GetTripsByUser(friend, _loggedInUser);

            Assert.Single(trips);
        }
    }
}
