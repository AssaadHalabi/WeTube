import React, { useEffect, useState } from 'react';
import VideoCard from '../../VideoCard/VideoCard';
import {Helmet} from "react-helmet";

import './SubscriptionPage.css';

function SubscriptionPage() {
  const [content, setContent] = useState(null);
  const currentUser = JSON.parse(window.localStorage.getItem('CurrentUser'));

  useEffect(() => {
    if (currentUser != null) {
      let userId = currentUser?.id;
      let userSecret = currentUser?.secret;
  
      var raw = JSON.stringify({ UserId: userId, UserSecret: userSecret });
  
      var myHeaders = new Headers();
      myHeaders.append('Content-Type', 'application/json');
  
      var requestOptions = {
        method: 'POST',
        headers: myHeaders,
        body: raw,
        redirect: 'follow',
      };
  
      fetch(
        'https://youtube278.azurewebsites.net/api/video/from-subscriptions',
        requestOptions
      )
        .then((response) => response.json())
        .then((result) => setContent(result))
        .catch((error) => console.log('error', error));
    }
  }, []);

  if (currentUser == null) {
    return (
      <div>
        <h2>Subscriptions</h2>
        <h2>You are not logged in</h2>
      </div>
    )
  }

  return (
    <div className="subscriptions">
      <Helmet>
          <meta charSet="utf-8" />
          <title>WeTube - Subscriptions</title>
          <link rel="canonical" href="http://example.com" />
      </Helmet>
      <h2>Subscriptions</h2>
      <div className="subscriptions__videos">
        {content?.videos?.map((video) => {
          return (
            <VideoCard
              title={video.title}
              views={video.views.length}
              timestamp={video.uploadDate}
              channelId={video.author.id}
              channelImg={`https://youtube278.azurewebsites.net/api/channel/image-stream/${video.author.id}`}
              channel={video.author.name}
              image={`https://youtube278.azurewebsites.net/api/video/image-stream/${video.id}`}
              path={video.id}
              key={video.id}
            />
          );
        })}
      </div>
    </div>
  );
}

export default SubscriptionPage;
