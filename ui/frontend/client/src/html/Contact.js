import React, { useEffect, useState } from 'react';

function Contact() {

  return <div>{/*{message}*/}
    <h1>Contact Us</h1>
    <form>
        <input type="email" placeholder="Email"></input>
        <input type="tel" placeholder="(111)-222-3333"></input>
        <input type="text" placeholder="First Name"></input>
        <input type="text" placeholder="Last Name"></input>
        <input type="text" placeholder="Message..."></input>
        <input type="Submit"></input>
    </form>
  </div>;
}

export default Contact;
