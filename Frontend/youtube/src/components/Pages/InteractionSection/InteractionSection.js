import React, { useState } from 'react';
import ThumbUpIcon from '@material-ui/icons/ThumbUp';
import ThumbDownAltIcon from '@material-ui/icons/ThumbDownAlt';
import ShareIcon from '@material-ui/icons/Share';
import PlaylistAddIcon from '@material-ui/icons/PlaylistAdd';
import ReportIcon from '@material-ui/icons/Report';
import CommentList from '../CommentList/CommentList';
import SnackBar from '@material-ui/core/Snackbar';
import Slide from '@material-ui/core/Slide';

import IconButton from '@material-ui/core/IconButton';
import CloseIcon from '@material-ui/icons/Close';

import './InteractionSection.css';
import { Button } from '@material-ui/core';

function InteractionSection() {
  const [selectedThumb, setSelectedThumb] = useState(null);
  const [open, setOpen] = useState(false);
  const [transition, setTransition] = useState(undefined);

  const currentUser = JSON.parse(window.localStorage.getItem('CurrentUser'));

  //Liking/Dislinking/Subscribing methods
  function LikeVideo() {
    fetch('https://youtube278.azurewebsites.net/api/video/like/12', {
      method: 'PUT',
      headers: {
        'Access-Control-Allow-Origin': '*',
        'Content-Type': 'application/json',
        mode: 'no-cors',
      },
      body: JSON.stringify({
        userId: currentUser.id,
        userSecret: currentUser.secret,
      }),
    }).catch((error) => console.log(error));
  }

  function DislikeVideo() {
    fetch('https://youtube278.azurewebsites.net/api/video/undolike/12', {
      method: 'POST',
      headers: {
        'Access-Control-Allow-Origin': '*',
        'Content-Type': 'application/json',
        mode: 'no-cors',
      },
      body: JSON.stringify({
        userId: currentUser.id,
        userSecret: currentUser.secret,
      }),
    }).catch((error) => console.log(error));
  }

  function TransitionUp(props) {
    return <Slide {...props} direction="up" />;
  }

  const handleClick = (thumb, Transition) => {
    setTransition(() => Transition);
    if (thumb === 'thumbsUp') {
      LikeVideo();
      setSelectedThumb('Video Liked');
    } else if (thumb === 'thumbsDown') {
      DislikeVideo();
      setSelectedThumb('Video Disliked');
    } else if (thumb === 'subscribe') setSelectedThumb('Subscribe To Channel');
    setOpen(true);
  };
  const handleClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }

    setOpen(false);
  };

  return (
    <div className="interactions">
      <SnackBar
        anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
        open={open}
        autoHideDuration={4000}
        onClose={handleClose}
        message={selectedThumb}
        TransitionComponent={transition}
        action={
          <React.Fragment>
            <IconButton
              size="small"
              aria-label="close"
              color="inherit"
              onClick={handleClose}
            >
              <CloseIcon fontSize="small" />
            </IconButton>
          </React.Fragment>
        }
      />
      <div className="interactions__info">
        <div className="interactions__mainInteractionSection">
          <div className="interactions__title">
            <h4>Friends "Skidmark still got a way with the ladies"</h4>
            <p>830,000 views • Jan 29, 2013</p>
          </div>
          <div className="interactions__interactiveSection">
            <Button
              className="interactions__subscribe"
              size="small"
              variant="contained"
              onClick={() => handleClick('subscribe', TransitionUp)}
            >
              Subscribe
            </Button>
            <ThumbUpIcon
              className="interactions__thumbsUp"
              onClick={() => handleClick('thumbsUp', TransitionUp)}
            />
            <ThumbDownAltIcon
              className="interactions__thumbsDown"
              onClick={() => handleClick('thumbsDown', TransitionUp)}
            />
            <ShareIcon />
            <PlaylistAddIcon />
            <ReportIcon />
          </div>
        </div>
        <div className="interactions__commentSection">
          <div className="interactions__commentSection">
            <CommentList />
          </div>
        </div>
      </div>
    </div>
  );
}

export default InteractionSection;
